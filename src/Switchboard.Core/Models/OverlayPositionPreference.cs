namespace Switchboard.Core.Models;

public sealed record OverlayPositionPreference
{
    public string? MonitorDeviceName { get; init; }

    public OverlayAnchor Anchor { get; init; } = OverlayAnchor.Center;

    public double OffsetX { get; init; }

    public double OffsetY { get; init; }
}
