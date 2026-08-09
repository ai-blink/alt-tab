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
    public void Calculate_accounts_for_ui_scale_when_choosing_columns()
    {
        var smallerUi = Calculate(windowCount: 20, uiScale: 0.85);
        var largerUi = Calculate(windowCount: 20, uiScale: 1.2);

        Assert.True(smallerUi.Columns > largerUi.Columns);
        Assert.True(ToPhysicalSize(smallerUi.Width, 0.85) <= 1536);
        Assert.True(ToPhysicalSize(largerUi.Height, 1.2) <= 816);
    }

    [Fact]
    public void Calculate_respects_compact_maximum_columns()
    {
        var layout = SwitcherLayoutCalculator.Calculate(
            windowCount: 30,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            uiScale: 1.0,
            mode: SwitcherViewMode.Compact,
            sizingPolicy: SwitcherSizingPolicy.Auto,
            cardWidth: 224,
            cardHeight: 96,
            maximumColumns: 4,
            maximumRows: 6);

        Assert.InRange(layout.Columns, 1, 4);
    }

    [Fact]
    public void Calculate_uses_the_selected_compact_column_limit_when_it_fits()
    {
        var layout = SwitcherLayoutCalculator.Calculate(
            windowCount: 25,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            uiScale: 1.1,
            mode: SwitcherViewMode.Compact,
            sizingPolicy: SwitcherSizingPolicy.Auto,
            cardWidth: 224,
            cardHeight: 96,
            maximumColumns: 5,
            maximumRows: 5);

        Assert.Equal(5, layout.Columns);
        Assert.Equal(5, layout.Rows);
        Assert.False(layout.RequiresVerticalScroll);
        Assert.True(ToPhysicalSize(layout.Width, 1.1) <= 1536);
        Assert.True(ToPhysicalSize(layout.Height, 1.1) <= 816);
    }

    [Fact]
    public void Calculate_enables_scroll_when_compact_maximum_rows_is_exceeded()
    {
        var layout = SwitcherLayoutCalculator.Calculate(
            windowCount: 43,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            uiScale: 1.0,
            mode: SwitcherViewMode.Compact,
            sizingPolicy: SwitcherSizingPolicy.Auto,
            cardWidth: 224,
            cardHeight: 96,
            maximumColumns: 7,
            maximumRows: 6);

        Assert.InRange(layout.Columns, 1, 7);
        Assert.True(layout.Rows > 6);
        Assert.True(layout.RequiresVerticalScroll);
    }

    private static SwitcherLayout Calculate(int windowCount, double uiScale = 1.0) =>
        SwitcherLayoutCalculator.Calculate(
            windowCount,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            uiScale,
            SwitcherViewMode.Grid,
            SwitcherSizingPolicy.Auto,
            cardWidth: 274,
            cardHeight: 188);

    private static double ToPhysicalSize(double logicalSize, double uiScale) =>
        36 + (Math.Max(0, logicalSize - 36) * uiScale);
}
