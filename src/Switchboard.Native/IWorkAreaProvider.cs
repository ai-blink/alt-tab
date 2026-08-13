using Switchboard.Core.Models;

namespace Switchboard.Native;

public interface IWorkAreaProvider
{
    OverlayWorkArea GetWorkAreaForWindow(nint windowHandle);

    OverlayWorkArea GetPrimaryWorkArea();

    bool TryGetWorkArea(string? deviceName, out OverlayWorkArea workArea);
}
