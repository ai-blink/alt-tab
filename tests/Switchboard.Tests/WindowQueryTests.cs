using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class WindowQueryTests
{
    [Fact]
    public void Apply_filters_by_app_title_or_monitor()
    {
        var windows = CreateWindows();

        var result = WindowQuery.Apply(windows, "external", WindowSortMode.Recent);

        Assert.Single(result);
        Assert.Equal("Browser", result[0].AppName);
    }

    [Fact]
    public void Apply_sorts_favorites_before_recent_windows()
    {
        var windows = CreateWindows();

        var result = WindowQuery.Apply(windows, null, WindowSortMode.Favorites);

        Assert.Equal("Editor", result[0].AppName);
    }

    [Fact]
    public void Apply_does_not_cap_results_at_twenty_five_windows()
    {
        var now = DateTimeOffset.UtcNow;
        var windows = Enumerable.Range(1, 30)
            .Select(index => new WindowSnapshot(
                $"window-{index}",
                index,
                "App",
                $"Window {index}",
                "Primary",
                "APP",
                "#111111",
                false,
                false,
                now.AddSeconds(-index)))
            .ToArray();

        var result = WindowQuery.Apply(windows, null, WindowSortMode.Recent);

        Assert.Equal(30, result.Count);
    }

    private static WindowSnapshot[] CreateWindows()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new("editor", 1, "Editor", "Project", "Primary", "CODE", "#111111", false, true, now.AddMinutes(-20)),
            new("browser", 2, "Browser", "External reference", "EXT-1", "WEB", "#ffffff", false, false, now.AddMinutes(-1))
        ];
    }
}
