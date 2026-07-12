using System.Runtime.InteropServices;

namespace Switchboard.Native;

public static class ForegroundWindowPresenter
{
    private const int SwShownormal = 1;
    private const int SwRestore = 9;
    private const uint SwpNosize = 0x0001;
    private const uint SwpNomove = 0x0002;
    private const uint SwpNoactivate = 0x0010;
    private const uint SwpShowwindow = 0x0040;
    private static readonly nint HwndTopmost = new(-1);
    private static readonly nint HwndNotopmost = new(-2);

    public static nint GetCurrentWindow() => GetForegroundWindow();

    public static bool TryPresent(nint hwnd, bool keepTopmost)
    {
        if (hwnd == 0 || !IsWindow(hwnd))
        {
            return false;
        }

        var currentThread = GetCurrentThreadId();
        var foregroundThread = GetWindowThreadProcessId(GetForegroundWindow(), out _);
        var attachedToForeground = foregroundThread != 0 && foregroundThread != currentThread &&
            AttachThreadInput(currentThread, foregroundThread, true);

        try
        {
            _ = ShowWindow(hwnd, SwShownormal);

            if (!keepTopmost)
            {
                const uint pulseFlags = SwpNomove | SwpNosize | SwpNoactivate | SwpShowwindow;
                _ = SetWindowPos(hwnd, HwndTopmost, 0, 0, 0, 0, pulseFlags);
                _ = SetWindowPos(hwnd, HwndNotopmost, 0, 0, 0, 0, pulseFlags);
            }

            _ = BringWindowToTop(hwnd);

            if (SetForegroundWindow(hwnd) || GetForegroundWindow() == hwnd)
            {
                return true;
            }

            SwitchToThisWindow(hwnd, true);
            return GetForegroundWindow() == hwnd;
        }
        finally
        {
            if (attachedToForeground)
            {
                _ = AttachThreadInput(currentThread, foregroundThread, false);
            }
        }
    }

    public static bool TryRestore(nint hwnd)
    {
        if (hwnd == 0 || !IsWindow(hwnd))
        {
            return false;
        }

        _ = ShowWindow(hwnd, SwRestore);
        var currentThread = GetCurrentThreadId();
        var foregroundThread = GetWindowThreadProcessId(GetForegroundWindow(), out _);
        var targetThread = GetWindowThreadProcessId(hwnd, out _);
        var attachedToForeground = foregroundThread != 0 && foregroundThread != currentThread &&
            AttachThreadInput(currentThread, foregroundThread, true);
        var attachedToTarget = targetThread != 0 && targetThread != currentThread && targetThread != foregroundThread &&
            AttachThreadInput(currentThread, targetThread, true);

        try
        {
            _ = BringWindowToTop(hwnd);

            if (SetForegroundWindow(hwnd) || GetForegroundWindow() == hwnd)
            {
                return true;
            }

            SwitchToThisWindow(hwnd, true);
            return GetForegroundWindow() == hwnd;
        }
        finally
        {
            if (attachedToTarget)
            {
                _ = AttachThreadInput(currentThread, targetThread, false);
            }

            if (attachedToForeground)
            {
                _ = AttachThreadInput(currentThread, foregroundThread, false);
            }
        }
    }

    [DllImport("user32.dll")]
    private static extern bool IsWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("user32.dll")]
    private static extern void SwitchToThisWindow(nint hWnd, bool fAltTab);
}
