using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.App.Localization;

// Owns the single merged Strings.{code}.xaml dictionary; XAML reads it through DynamicResource.
// It also stamps each window's xml:lang so WPF picks CJK glyphs for the active language
// (Han characters shared by Chinese and Japanese render differently per language).
public static class AppLocalizer
{
    private const string DictionaryPrefix = "/Localization/Strings.";

    private static XmlLanguage windowLanguage = XmlLanguage.GetLanguage(AppLanguageResolver.English);
    private static bool isWindowHookRegistered;

    public static void Apply(AppLanguage language)
    {
        if (System.Windows.Application.Current is not { } application)
        {
            return;
        }

        var code = AppLanguageResolver.ResolveCode(language, CultureInfo.CurrentUICulture);
        ReplaceStringDictionary(application.Resources.MergedDictionaries, code);

        windowLanguage = XmlLanguage.GetLanguage(code);
        RegisterWindowHook();

        foreach (Window window in application.Windows)
        {
            window.Language = windowLanguage;
        }
    }

    public static string Get(string key) =>
        System.Windows.Application.Current?.TryFindResource(key) as string ?? key;

    private static void ReplaceStringDictionary(Collection<ResourceDictionary> dictionaries, string code)
    {
        var replacement = new ResourceDictionary
        {
            Source = new Uri($"{DictionaryPrefix}{code}.xaml", UriKind.Relative)
        };

        var current = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source?.OriginalString.StartsWith(DictionaryPrefix, StringComparison.Ordinal) == true);

        if (current is null)
        {
            dictionaries.Add(replacement);
        }
        else if (current.Source!.OriginalString != replacement.Source.OriginalString)
        {
            dictionaries[dictionaries.IndexOf(current)] = replacement;
        }
    }

    // Windows opened later (the settings modal) take the current language when they load.
    private static void RegisterWindowHook()
    {
        if (isWindowHookRegistered)
        {
            return;
        }

        isWindowHookRegistered = true;
        EventManager.RegisterClassHandler(
            typeof(Window),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler((sender, _) => ((Window)sender).Language = windowLanguage));
    }
}
