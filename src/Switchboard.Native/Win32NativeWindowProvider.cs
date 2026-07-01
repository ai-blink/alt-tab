using Switchboard.Core.Models;

namespace Switchboard.Native;

public sealed class Win32NativeWindowProvider : INativeWindowProvider
{
    public IReadOnlyList<WindowSnapshot> GetTopLevelWindows()
    {
        // Native enumeration lands here next: EnumWindows -> DWM thumbnails -> foreground activation.
        return [];
    }
}
