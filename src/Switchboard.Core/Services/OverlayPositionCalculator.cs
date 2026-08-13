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

    public static OverlayAnchor ToAnchor(OverlayPlacement placement) => placement switch
    {
        OverlayPlacement.TopLeft => OverlayAnchor.TopLeft,
        OverlayPlacement.TopRight => OverlayAnchor.TopRight,
        OverlayPlacement.BottomLeft => OverlayAnchor.BottomLeft,
        OverlayPlacement.BottomRight => OverlayAnchor.BottomRight,
        _ => OverlayAnchor.Center
    };

    public static OverlayPositionPreference Capture(
        OverlayWorkArea workArea,
        OverlayPosition position,
        double overlayWidth,
        double overlayHeight)
    {
        var constrainedPosition = Constrain(workArea, position, overlayWidth, overlayHeight);
        var anchor = FindNearestAnchor(workArea, constrainedPosition, overlayWidth, overlayHeight);
        var anchorPosition = Calculate(workArea, overlayWidth, overlayHeight, anchor);

        return new OverlayPositionPreference
        {
            MonitorDeviceName = workArea.DeviceName,
            Anchor = anchor,
            OffsetX = constrainedPosition.Left - anchorPosition.Left,
            OffsetY = constrainedPosition.Top - anchorPosition.Top
        };
    }

    public static OverlayPosition Calculate(
        OverlayWorkArea workArea,
        double overlayWidth,
        double overlayHeight,
        OverlayPositionPreference preference)
    {
        var anchorPosition = Calculate(workArea, overlayWidth, overlayHeight, preference.Anchor);
        var position = new OverlayPosition(
            anchorPosition.Left + preference.OffsetX,
            anchorPosition.Top + preference.OffsetY);

        return Constrain(workArea, position, overlayWidth, overlayHeight);
    }

    public static OverlayPosition Calculate(
        OverlayWorkArea workArea,
        double overlayWidth,
        double overlayHeight,
        OverlayAnchor anchor)
    {
        var horizontalSpace = Math.Max(0, workArea.Width - overlayWidth);
        var verticalSpace = Math.Max(0, workArea.Height - overlayHeight);
        var horizontalEdge = Math.Min(EdgeMargin, horizontalSpace);
        var verticalEdge = Math.Min(EdgeMargin, verticalSpace);

        var left = anchor is OverlayAnchor.TopRight or OverlayAnchor.MiddleRight or OverlayAnchor.BottomRight
            ? workArea.Left + horizontalSpace - horizontalEdge
            : anchor is OverlayAnchor.TopCenter or OverlayAnchor.Center or OverlayAnchor.BottomCenter
                ? workArea.Left + (horizontalSpace / 2)
                : workArea.Left + horizontalEdge;
        var top = anchor is OverlayAnchor.BottomLeft or OverlayAnchor.BottomCenter or OverlayAnchor.BottomRight
            ? workArea.Top + verticalSpace - verticalEdge
            : anchor is OverlayAnchor.MiddleLeft or OverlayAnchor.Center or OverlayAnchor.MiddleRight
                ? workArea.Top + (verticalSpace / 2)
                : workArea.Top + verticalEdge;

        return new OverlayPosition(left, top);
    }

    public static OverlayPosition Constrain(
        OverlayWorkArea workArea,
        OverlayPosition position,
        double overlayWidth,
        double overlayHeight)
    {
        var horizontalSpace = Math.Max(0, workArea.Width - overlayWidth);
        var verticalSpace = Math.Max(0, workArea.Height - overlayHeight);
        var horizontalEdge = Math.Min(EdgeMargin, horizontalSpace);
        var verticalEdge = Math.Min(EdgeMargin, verticalSpace);

        var minimumLeft = workArea.Left + horizontalEdge;
        var minimumTop = workArea.Top + verticalEdge;
        var maximumLeft = Math.Max(minimumLeft, workArea.Right - overlayWidth - horizontalEdge);
        var maximumTop = Math.Max(minimumTop, workArea.Bottom - overlayHeight - verticalEdge);

        return new OverlayPosition(
            Math.Clamp(position.Left, minimumLeft, maximumLeft),
            Math.Clamp(position.Top, minimumTop, maximumTop));
    }

    private static OverlayAnchor FindNearestAnchor(
        OverlayWorkArea workArea,
        OverlayPosition position,
        double overlayWidth,
        double overlayHeight)
    {
        var centerX = (position.Left + (overlayWidth / 2) - workArea.Left) / Math.Max(1, workArea.Width);
        var centerY = (position.Top + (overlayHeight / 2) - workArea.Top) / Math.Max(1, workArea.Height);
        var horizontal = centerX < 1d / 3d
            ? 0
            : centerX > 2d / 3d
                ? 2
                : 1;
        var vertical = centerY < 1d / 3d
            ? 0
            : centerY > 2d / 3d
                ? 2
                : 1;

        return (vertical, horizontal) switch
        {
            (0, 0) => OverlayAnchor.TopLeft,
            (0, 1) => OverlayAnchor.TopCenter,
            (0, 2) => OverlayAnchor.TopRight,
            (1, 0) => OverlayAnchor.MiddleLeft,
            (1, 1) => OverlayAnchor.Center,
            (1, 2) => OverlayAnchor.MiddleRight,
            (2, 0) => OverlayAnchor.BottomLeft,
            (2, 1) => OverlayAnchor.BottomCenter,
            _ => OverlayAnchor.BottomRight
        };
    }
}

public readonly record struct OverlayPosition(double Left, double Top);
