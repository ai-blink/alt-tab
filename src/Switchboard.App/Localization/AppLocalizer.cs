using System.Globalization;
using System.Windows;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.App.Localization;

// Owns the single merged Strings.{code}.xaml dictionary; XAML reads it through DynamicResource.
public static class AppLocalizer
{
    private const string DictionaryPrefix = "/Localization/Strings.";

    public static void Apply(AppLanguage language)
    {
        if (System.Windows.Application.Current is not { } application)
        {
            return;
        }

        var code = AppLanguageResolver.ResolveCode(language, CultureInfo.CurrentUICulture);
        var dictionaries = application.Resources.MergedDictionaries;
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

    public static string Get(string key) =>
        System.Windows.Application.Current?.TryFindResource(key) as string ?? key;
}
