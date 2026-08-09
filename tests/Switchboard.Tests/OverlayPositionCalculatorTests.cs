using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class OverlayPositionCalculatorTests
{
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
}
