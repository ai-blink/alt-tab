using System.Runtime.InteropServices;
using System.Text;

namespace Switchboard.Native;

public static class WindowIconSource
{
    private const uint WmGetIcon = 0x007F;
    private const int IconSmall = 0;
    private const int IconBig = 1;
    private const int IconSmall2 = 2;
    private const int GclpHicon = -14;
    private const int GclpHiconSm = -34;
    private const uint SmtoAbortIfHung = 0x0002;
    private const uint IconMessageTimeoutMs = 50;
    private const uint ProcessQueryLimitedInformation = 0x1000;

    // Returns an icon owned by the window or its class; callers must copy it and must not destroy it.
    // The message has a short timeout so a hung window cannot stall the overlay.
    public static nint TryGetSharedIcon(nint hwnd)
    {
        foreach (var kind in new[] { IconBig, IconSmall2, IconSmall })
        {
            if (SendMessageTimeout(hwnd, WmGetIcon, kind, 0, SmtoAbortIfHung, IconMessageTimeoutMs, out var icon) != 0 && icon != 0)
            {
                return icon;
            }
        }

        var classIcon = GetClassLongPtr(hwnd, GclpHicon);
        return classIcon != 0 ? classIcon : GetClassLongPtr(hwnd, GclpHiconSm);
    }

    // Fallback source when the window exposes no icon (common for elevated windows, whose messages UIPI blocks).
    public static string? TryGetProcessImagePath(nint hwnd)
    {
        _ = GetWindowThreadProcessId(hwnd, out var processId);
        var process = OpenProcess(ProcessQueryLimitedInformation, false, processId);

        if (process == 0)
        {
            return null;
        }

        try
        {
            var builder = new StringBuilder(1024);
            var size = builder.Capacity;
            return QueryFullProcessImageName(process, 0, builder, ref size) ? builder.ToString(0, size) : null;
        }
        finally
        {
            _ = CloseHandle(process);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SendMessageTimeout(nint hwnd, uint message, nint wParam, nint lParam, uint flags, uint timeout, out nint result);

    [DllImport("user32.dll", EntryPoint = "GetClassLongPtrW")]
    private static extern nint GetClassLongPtr(nint hwnd, int index);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hwnd, out uint processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(uint access, bool inheritHandle, uint processId);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool QueryFullProcessImageName(nint process, int flags, StringBuilder exeName, ref int size);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint handle);
}
