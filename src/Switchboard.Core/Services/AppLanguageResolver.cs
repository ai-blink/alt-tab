using System.Globalization;
using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class AppLanguageResolver
{
    public const string Korean = "ko";
    public const string English = "en";
    public const string SimplifiedChinese = "zh-Hans";
    public const string Japanese = "ja";

    // Auto follows the Windows display language; every Chinese variant uses Simplified Chinese,
    // and unsupported languages fall back to English.
    public static string ResolveCode(AppLanguage language, CultureInfo uiCulture) => language switch
    {
        AppLanguage.Korean => Korean,
        AppLanguage.English => English,
        AppLanguage.SimplifiedChinese => SimplifiedChinese,
        AppLanguage.Japanese => Japanese,
        _ => uiCulture.TwoLetterISOLanguageName switch
        {
            "ko" => Korean,
            "zh" => SimplifiedChinese,
            "ja" => Japanese,
            _ => English
        }
    };
}
