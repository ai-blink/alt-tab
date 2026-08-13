namespace Switchboard.Core.Models;

public readonly record struct OverlayWorkArea(
    string DeviceName,
    double Left,
    double Top,
    double Width,
    double Height)
{
    public double Right => Left + Math.Max(0, Width);

    public double Bottom => Top + Math.Max(0, Height);
}
