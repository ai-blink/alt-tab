using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public sealed class DemoWindowCatalog : IWindowCatalog
{
    public IReadOnlyList<WindowSnapshot> GetOpenWindows()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new("vscode", 0x1001, "VS Code", "app.tsx - Switchboard Project", "Primary", "CODE", "#1E1E1E", false, true, now.AddMinutes(-1)),
            new("terminal", 0x1002, "Terminal", "yarn start - build process", "Primary", "CLI", "#050505", true, false, now.AddMinutes(-2)),
            new("chrome", 0x1003, "Google Chrome", "Figma - Dashboard UI Components", "EXT-1", "WEB", "#EAF2FF", false, false, now.AddMinutes(-7)),
            new("figma", 0x1004, "Figma", "V2 Design System Library", "EXT-2", "FIG", "#F6F3F2", false, true, now.AddMinutes(-10)),
            new("explorer", 0x1005, "File Explorer", @"C:\Projects\switchboard-ui\src", "Primary", "DIR", "#F0EDED", false, false, now.AddMinutes(-18)),
            new("teams", 0x1006, "Microsoft Teams", "Engineering Sync (5 people)", "EXT-1", "MEET", "#E5F1FF", false, false, now.AddMinutes(-25))
        ];
    }
}
