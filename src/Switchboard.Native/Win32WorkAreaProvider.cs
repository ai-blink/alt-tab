using System.Runtime.InteropServices;
using Switchboard.Core.Models;

namespace Switchboard.Native;

public sealed class Win32WorkAreaProvider : IWorkAreaProvider
{
    private const uint MonitorDefaultToPrimary = 1;
    private const uint MonitorDefaultToNearest = 2;

    public OverlayWorkArea GetWorkAreaForWindow(nint windowHandle)
    {
        var monitor = windowHandle == 0
            ? 0
            : MonitorFromWindow(windowHandle, MonitorDefaultToNearest);

        return TryReadWorkArea(monitor, out var workArea)
            ? workArea
            : GetPrimaryWorkArea();
    }

    public OverlayWorkArea GetPrimaryWorkArea()
    {
        var monitor = MonitorFromPoint(new NativePoint(), MonitorDefaultToPrimary);

        return TryReadWorkArea(monitor, out var workArea)
            ? workArea
            : new OverlayWorkArea("DISPLAY", 0, 0, 1_024, 768);
    }

    public bool TryGetWorkArea(string? deviceName, out OverlayWorkArea workArea)
    {
        workArea = default;

        if (string.IsNullOrWhiteSpace(deviceName))
        {
            return false;
        }

        OverlayWorkArea? match = null;
        _ = EnumDisplayMonitors(
            0,
            0,
            (nint monitor, nint deviceContext, ref NativeRect monitorBounds, nint callbackData) =>
            {
                if (!TryReadWorkArea(monitor, out var candidate) ||
                    !string.Equals(candidate.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                match = candidate;
                return false;
            },
            0);

        if (match is not { } found)
        {
            return false;
        }

        workArea = found;
        return true;
    }

    private static bool TryReadWorkArea(nint monitor, out OverlayWorkArea workArea)
    {
        workArea = default;
        var info = new MonitorInfoEx
        {
            Size = Marshal.SizeOf<MonitorInfoEx>()
        };

        if (monitor == 0 || !GetMonitorInfo(monitor, ref info))
        {
            return false;
        }

        var deviceName = NormalizeDeviceName(info.DeviceName);
        var bounds = info.WorkArea;
        workArea = new OverlayWorkArea(
            deviceName,
            bounds.Left,
            bounds.Top,
            Math.Max(0, bounds.Right - bounds.Left),
            Math.Max(0, bounds.Bottom - bounds.Top));
        return true;
    }

    private static string NormalizeDeviceName(string deviceName)
    {
        var trimmed = deviceName.TrimEnd('\0');
        var slashIndex = trimmed.LastIndexOf('\\');
        return slashIndex >= 0 && slashIndex < trimmed.Length - 1
            ? trimmed[(slashIndex + 1)..]
            : string.IsNullOrWhiteSpace(trimmed)
                ? "DISPLAY"
                : trimmed;
    }

    private delegate bool MonitorEnumProc(nint monitor, nint hdc, ref NativeRect monitorRect, nint data);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint windowHandle, uint flags);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromPoint(NativePoint point, uint flags);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(nint hdc, nint clipRect, MonitorEnumProc callback, nint data);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(nint monitor, ref MonitorInfoEx info);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfoEx
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect WorkArea;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }
}
