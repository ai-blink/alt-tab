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
        var scaledDown = Calculate(windowCount: 20, appScale: 0.8);
        var scaledUp = Calculate(windowCount: 20, appScale: 2.0);

        Assert.True(scaledDown.Columns > scaledUp.Columns);
        Assert.True(SwitcherLayoutCalculator.ScaleLayoutDimension(scaledDown.Width, 0.8) <= 1536);
        Assert.True(SwitcherLayoutCalculator.ScaleLayoutDimension(scaledUp.Height, 2.0) <= 816);
    }

    [Theory]
    [InlineData(0.8)]
    [InlineData(1.0)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    public void Calculate_keeps_thumbnail_1_2_columns_aligned_with_content_width(double appScale)
    {
        const double cardWidth = 329;
        const double itemWidth = cardWidth + 6 + 8;
        var layout = SwitcherLayoutCalculator.Calculate(
            windowCount: 20,
            workAreaWidth: 1536,
            workAreaHeight: 816,
            appScale,
            SwitcherViewMode.Grid,
            SwitcherSizingPolicy.Auto,
            cardWidth,
            cardHeight: 220);
        var availableContentWidth = layout.Width - 36 - 28 - 24;
        var unusedContentWidth = availableContentWidth - (layout.Columns * itemWidth);

        Assert.Equal(layout.Columns, (int)Math.Floor(availableContentWidth / itemWidth));
        Assert.InRange(unusedContentWidth, 0, itemWidth - 0.001);
    }

    [Fact]
    public void Minimum_grid_row_at_200_percent_expands_compact_height_beyond_fixed_ratio()
    {
        var minimumLogicalHeight = SwitcherLayoutCalculator.CalculateMinimumLayoutHeight(
            SwitcherViewMode.Grid,
            cardHeight: 220);
        var minimumPhysicalHeight = SwitcherLayoutCalculator.ScaleLayoutDimension(
            minimumLogicalHeight,
            appScale: 2.0);

        Assert.Equal(398, minimumLogicalHeight);
        Assert.Equal(760, minimumPhysicalHeight);
        Assert.True(minimumPhysicalHeight > 816 * 0.65);
        Assert.True(minimumPhysicalHeight <= 816);
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
