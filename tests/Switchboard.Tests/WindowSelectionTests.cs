using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class WindowSelectionTests
{
    [Fact]
    public void Move_cycles_forward_and_backward()
    {
        var windows = CreateWindows();

        Assert.Equal("second", WindowSelection.Move(windows, windows[0], 1)?.Id);
        Assert.Equal("third", WindowSelection.Move(windows, windows[0], -1)?.Id);
    }

    [Fact]
    public void EnsureVisible_keeps_matching_window_by_id()
    {
        var windows = CreateWindows();
        var staleSelection = windows[1] with { Title = "Old title" };

        var selectedWindow = WindowSelection.EnsureVisible(windows, staleSelection);

        Assert.Same(windows[1], selectedWindow);
    }

    private static WindowSnapshot[] CreateWindows()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new("first", 1, "Editor", "Project", "Primary", "CODE", "#111111", false, false, now),
            new("second", 2, "Browser", "Reference", "Primary", "WEB", "#222222", false, false, now),
            new("third", 3, "Terminal", "Build", "Primary", "TERM", "#333333", false, false, now)
        ];
    }
}
