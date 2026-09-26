using System.Globalization;
using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

// Short app label shown beside the window icon on a card. The title carries the meaning, so the
// label is hidden when it would only repeat the start of the title ("Hermes" / "Hermes 비서 챗").
public static class AppNameLabel
{
    public const string Ellipsis = "…";

    public static int ToMaxLength(AppNameLengthPreset preset) => preset switch
    {
        AppNameLengthPreset.Five => 5,
        AppNameLengthPreset.Seven => 7,
        _ => 6
    };

    public static string Format(string? appName, string? title, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            return string.Empty;
        }

        var name = appName.Trim();

        if (!string.IsNullOrWhiteSpace(title) &&
            title.TrimStart().StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
        {
            return string.Empty;
        }

        // Count user-perceived characters so surrogate pairs and combining marks are never split.
        var text = new StringInfo(name);

        return text.LengthInTextElements <= maxLength
            ? name
            : text.SubstringByTextElements(0, maxLength) + Ellipsis;
    }
}
