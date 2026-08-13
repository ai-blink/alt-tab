using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class OverlayPositionCalculatorTests
{
    private static readonly OverlayWorkArea WorkArea = new("DISPLAY1", 100, 200, 1_200, 800);

    [Theory]
    [InlineData(OverlayPlacement.BottomLeft, 118d, 682d)]
    [InlineData(OverlayPlacement.BottomRight, 782d, 682d)]
    [InlineData(OverlayPlacement.TopLeft, 118d, 218d)]
    [InlineData(OverlayPlacement.TopRight, 782d, 218d)]
    [InlineData(OverlayPlacement.Center, 450d, 450d)]
    public void Calculate_places_overlay_at_the_requested_work_area_position(
        OverlayPlacement placement,
        double expectedLeft,
        double expectedTop)
    {
        var position = OverlayPositionCalculator.Calculate(
            workAreaLeft: 100,
            workAreaTop: 200,
            workAreaWidth: 1_200,
            workAreaHeight: 800,
            overlayWidth: 500,
            overlayHeight: 300,
            placement);

        Assert.Equal(expectedLeft, position.Left);
        Assert.Equal(expectedTop, position.Top);
    }

    [Theory]
    [InlineData(OverlayAnchor.TopLeft, 118d, 218d)]
    [InlineData(OverlayAnchor.TopCenter, 450d, 218d)]
    [InlineData(OverlayAnchor.TopRight, 782d, 218d)]
    [InlineData(OverlayAnchor.MiddleLeft, 118d, 450d)]
    [InlineData(OverlayAnchor.Center, 450d, 450d)]
    [InlineData(OverlayAnchor.MiddleRight, 782d, 450d)]
    [InlineData(OverlayAnchor.BottomLeft, 118d, 682d)]
    [InlineData(OverlayAnchor.BottomCenter, 450d, 682d)]
    [InlineData(OverlayAnchor.BottomRight, 782d, 682d)]
    public void Calculate_places_overlay_at_each_anchor(
        OverlayAnchor anchor,
        double expectedLeft,
        double expectedTop)
    {
        var position = OverlayPositionCalculator.Calculate(WorkArea, 500, 300, anchor);

        Assert.Equal(expectedLeft, position.Left);
        Assert.Equal(expectedTop, position.Top);
    }

    [Fact]
    public void Capture_and_restore_keep_the_bottom_right_edge_when_overlay_grows()
    {
        var preference = OverlayPositionCalculator.Capture(
            WorkArea,
            new OverlayPosition(752, 662),
            overlayWidth: 500,
            overlayHeight: 300);

        var restored = OverlayPositionCalculator.Calculate(WorkArea, 600, 400, preference);

        Assert.Equal(OverlayAnchor.BottomRight, preference.Anchor);
        Assert.Equal(1_252, restored.Left + 600);
        Assert.Equal(962, restored.Top + 400);
    }

    [Fact]
    public void Calculate_constrains_a_saved_position_inside_the_available_work_area()
    {
        var position = OverlayPositionCalculator.Calculate(
            WorkArea,
            overlayWidth: 500,
            overlayHeight: 300,
            new OverlayPositionPreference
            {
                MonitorDeviceName = "DISPLAY1",
                Anchor = OverlayAnchor.TopLeft,
                OffsetX = -200,
                OffsetY = 900
            });

        Assert.Equal(new OverlayPosition(118, 682), position);
    }
}
