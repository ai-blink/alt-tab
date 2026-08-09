using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class SwitcherLayoutCalculatorTests
{
    [Fact]
    public void Calculate_grows_with_window_count_until_work_area_is_full()
    {
        var small = Calculate(windowCount: 4);
        var large = Calculate(windowCount: 30);

        Assert.True(large.Rows > small.Rows);
        Assert.True(large.Height > small.Height);
        Assert.True(large.Rows * large.Columns >= 30);
        Assert.True(large.RequiresVerticalScroll);
    }

    [Fact]
    public void Calculate_accounts_for_overlay_scale_when_choosing_columns()
    {
        var scaledDown = Calculate(windowCount: 20, appScale: 0.5);
        var fullScale = Calculate(windowCount: 20, appScale: 1.0);

        Assert.True(scaledDown.Columns > fullScale.Columns);
        Assert.True(scaledDown.Width * 0.5 <= 1536);
        Assert.True(scaledDown.Height * 0.5 <= 816);
    }

    private static SwitcherLayout Calculate(int windowCount, double appScale = 0.9) =>
        SwitcherLayoutCalculator.Calculate(
            windowCount,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            appScale,
            SwitcherViewMode.Grid,
            SwitcherSizingPolicy.Auto,
            cardWidth: 274,
            cardHeight: 188);
}
