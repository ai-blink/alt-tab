using System.Globalization;
using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class AppLanguageResolver
{
    public const string Korean = "ko";
    public const string English = "en";

    // Auto follows the Windows display language; unsupported languages fall back to English.
    public static string ResolveCode(AppLanguage language, CultureInfo uiCulture) => language switch
    {
        AppLanguage.Korean => Korean,
        AppLanguage.English => English,
        _ => uiCulture.TwoLetterISOLanguageName == Korean ? Korean : English
    };
}
