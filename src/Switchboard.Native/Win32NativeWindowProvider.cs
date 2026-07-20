using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Native;

public sealed class Win32NativeWindowProvider : INativeWindowProvider, IWindowCatalog, IWindowActivator, IWindowCloser
{
    private const int GwlExStyle = -20;
    private const uint GwOwner = 4;
    private const long WsExToolWindow = 0x00000080L;
    private const long WsExAppWindow = 0x00040000L;
    private const int DwmwaCloaked = 14;
    private const int SwRestore = 9;
    private const uint WmClose = 0x0010;

    public IReadOnlyList<WindowSnapshot> GetOpenWindows() => GetTopLevelWindows();

    public bool TryActivate(WindowSnapshot window)
    {
        if (window.Handle == 0 || !IsWindow(window.Handle))
        {
            return false;
        }

        if (IsIconic(window.Handle))
        {
            _ = ShowWindow(window.Handle, SwRestore);
        }

        _ = BringWindowToTop(window.Handle);
        var foregroundRequested = SetForegroundWindow(window.Handle);

        return foregroundRequested || GetForegroundWindow() == window.Handle;
    }

    public bool TryClose(WindowSnapshot window) =>
        window.Handle != 0 &&
        IsWindow(window.Handle) &&
        PostMessage(window.Handle, WmClose, 0, 0);

    public IReadOnlyList<WindowSnapshot> GetTopLevelWindows()
    {
        var windows = new List<WindowSnapshot>();
        var shellWindow = GetShellWindow();
        var foregroundWindow = GetForegroundWindow();
        var currentProcessId = Environment.ProcessId;
        var now = DateTimeOffset.UtcNow;
        var index = 0;

        EnumWindows((hwnd, _) =>
        {
            if (!IsCandidateWindow(hwnd, shellWindow, currentProcessId))
            {
                return true;
            }

            var title = GetWindowTitle(hwnd);

            if (string.IsNullOrWhiteSpace(title))
            {
                return true;
            }

            var processId = GetWindowProcessId(hwnd);
            var appName = GetProcessName(processId);
            var monitorName = GetMonitorName(hwnd);
            var label = CreateLabel(appName);

            windows.Add(new WindowSnapshot(
                Id: hwnd.ToString("X"),
                Handle: hwnd,
                AppName: appName,
                Title: title,
                MonitorName: monitorName,
                ThumbnailLabel: label,
                ThumbnailBrush: CreateBrush(appName),
                IsActive: hwnd == foregroundWindow,
                IsFavorite: false,
                LastActivatedAt: now.AddMilliseconds(-index)));

            index++;
            return true;
        }, 0);

        return windows
            .OrderByDescending(window => window.IsActive)
            .ThenByDescending(window => window.LastActivatedAt)
            .ToList();
    }

    private static bool IsCandidateWindow(nint hwnd, nint shellWindow, int currentProcessId)
    {
        if (hwnd == 0 || hwnd == shellWindow || !IsWindowVisible(hwnd) || IsWindowCloaked(hwnd))
        {
            return false;
        }

        if (GetWindowProcessId(hwnd) == currentProcessId)
        {
            return false;
        }

        var exStyle = GetWindowLongPtr(hwnd, GwlExStyle).ToInt64();
        var owner = GetWindow(hwnd, GwOwner);

        if ((exStyle & WsExToolWindow) == WsExToolWindow)
        {
            return false;
        }

        if (owner != 0 && (exStyle & WsExAppWindow) != WsExAppWindow)
        {
            return false;
        }

        return true;
    }

    private static bool IsWindowCloaked(nint hwnd)
    {
        var cloaked = 0;
        var result = DwmGetWindowAttribute(hwnd, DwmwaCloaked, ref cloaked, Marshal.SizeOf<int>());
        return result == 0 && cloaked != 0;
    }

    private static string GetWindowTitle(nint hwnd)
    {
        var length = GetWindowTextLength(hwnd);

        if (length <= 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(length + 1);
        _ = GetWindowText(hwnd, builder, builder.Capacity);
        return builder.ToString().Trim();
    }

    private static int GetWindowProcessId(nint hwnd)
    {
        _ = GetWindowThreadProcessId(hwnd, out var processId);
        return (int)processId;
    }

    private static string GetProcessName(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return string.IsNullOrWhiteSpace(process.ProcessName)
                ? "Window"
                : process.ProcessName;
        }
        catch
        {
            return "Window";
        }
    }

    private static string GetMonitorName(nint hwnd)
    {
        var monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
        var info = new MonitorInfoEx();
        info.Size = Marshal.SizeOf<MonitorInfoEx>();

        if (monitor == 0 || !GetMonitorInfo(monitor, ref info))
        {
            return "Display";
        }

        var deviceName = info.DeviceName.TrimEnd('\0');
        var slashIndex = deviceName.LastIndexOf('\\');
        return slashIndex >= 0 && slashIndex < deviceName.Length - 1
            ? deviceName[(slashIndex + 1)..]
            : deviceName;
    }

    private static string CreateLabel(string appName)
    {
        var letters = new string(appName.Where(char.IsLetterOrDigit).Take(4).ToArray());
        return string.IsNullOrWhiteSpace(letters)
            ? "WIN"
            : letters.ToUpperInvariant();
    }

    private static string CreateBrush(string appName)
    {
        var colors = new[]
        {
            "#1F2937",
            "#0F172A",
            "#243B53",
            "#293241",
            "#343A40",
            "#334155",
            "#203A43",
            "#2D3748"
        };

        var hash = Math.Abs(StringComparer.OrdinalIgnoreCase.GetHashCode(appName));
        return colors[hash % colors.Length];
    }

    private delegate bool EnumWindowsProc(nint hwnd, nint lParam);

    private const uint MonitorDefaultToNearest = 2;

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(nint hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern nint GetShellWindow();

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern nint GetWindow(nint hWnd, uint uCmd);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfoEx
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
