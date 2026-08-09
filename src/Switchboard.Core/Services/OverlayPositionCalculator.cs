using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class OverlayPositionCalculator
{
    private const double EdgeMargin = 18;

    public static OverlayPosition Calculate(
        double workAreaLeft,
        double workAreaTop,
        double workAreaWidth,
        double workAreaHeight,
        double overlayWidth,
        double overlayHeight,
        OverlayPlacement placement)
    {
        var horizontalSpace = Math.Max(0, workAreaWidth - overlayWidth);
        var verticalSpace = Math.Max(0, workAreaHeight - overlayHeight);
        var horizontalEdge = Math.Min(EdgeMargin, horizontalSpace);
        var verticalEdge = Math.Min(EdgeMargin, verticalSpace);
        var left = placement is OverlayPlacement.BottomRight or OverlayPlacement.TopRight
            ? workAreaLeft + horizontalSpace - horizontalEdge
            : placement == OverlayPlacement.Center
                ? workAreaLeft + (horizontalSpace / 2)
                : workAreaLeft + horizontalEdge;
        var top = placement is OverlayPlacement.BottomLeft or OverlayPlacement.BottomRight
            ? workAreaTop + verticalSpace - verticalEdge
            : placement == OverlayPlacement.Center
                ? workAreaTop + (verticalSpace / 2)
                : workAreaTop + verticalEdge;

        return new OverlayPosition(left, top);
    }
}

public readonly record struct OverlayPosition(double Left, double Top);
