using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class LocalizationTests
{
    private static readonly XNamespace Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
    private static readonly string[] Languages = ["ko", "en"];

    [Fact]
    public void Every_language_dictionary_defines_the_same_keys()
    {
        var korean = LoadKeys("ko");

        foreach (var language in Languages)
        {
            Assert.Equal(korean.Order(), LoadKeys(language).Order());
        }
    }

    [Theory]
    [InlineData("MainWindow.xaml", @"\{DynamicResource ((?:Main|Settings|Position|Appearance|Behavior)\.[\w.]+)\}")]
    [InlineData("SettingsWindow.xaml", @"\{DynamicResource ((?:Main|Settings|Position|Appearance|Behavior)\.[\w.]+)\}")]
    [InlineData("ViewModels/MainWindowViewModel.cs", @"""(Position\.Anchor\.\w+)""")]
    public void Every_referenced_string_key_exists(string relativePath, string pattern)
    {
        var source = File.ReadAllText(Path.Combine(AppSourceDirectory(), relativePath));
        var referenced = Regex.Matches(source, pattern).Select(match => match.Groups[1].Value).Distinct().ToList();
        var defined = LoadKeys("en");

        Assert.NotEmpty(referenced);
        Assert.All(referenced, key => Assert.Contains(key, defined));
    }

    [Theory]
    [InlineData(AppLanguage.Auto, "ko-KR", "ko")]
    [InlineData(AppLanguage.Auto, "en-US", "en")]
    [InlineData(AppLanguage.Auto, "ja-JP", "en")]
    [InlineData(AppLanguage.Auto, "zh-CN", "en")]
    [InlineData(AppLanguage.Korean, "en-US", "ko")]
    [InlineData(AppLanguage.English, "ko-KR", "en")]
    public void Language_resolves_from_setting_then_windows_display_language(
        AppLanguage language,
        string uiCulture,
        string expectedCode)
    {
        Assert.Equal(expectedCode, AppLanguageResolver.ResolveCode(language, CultureInfo.GetCultureInfo(uiCulture)));
    }

    private static HashSet<string> LoadKeys(string language)
    {
        var document = XDocument.Load(Path.Combine(AppSourceDirectory(), "Localization", $"Strings.{language}.xaml"));

        return document.Root!
            .Elements()
            .Select(element => (string?)element.Attribute(Xaml + "Key"))
            .OfType<string>()
            .ToHashSet();
    }

    private static string AppSourceDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Switchboard.slnx")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine(directory!.FullName, "src", "Switchboard.App");
    }
}
