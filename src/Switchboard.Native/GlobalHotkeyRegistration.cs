using System.Runtime.InteropServices;
using Switchboard.Core.Models;

namespace Switchboard.Native;

public sealed class GlobalHotkeyRegistration : IDisposable
{
    private readonly nint hwnd;
    private readonly int id;
    private bool isRegistered;

    private GlobalHotkeyRegistration(nint hwnd, int id)
    {
        this.hwnd = hwnd;
        this.id = id;
    }

    public int ErrorCode { get; private set; }

    public bool IsRegistered => isRegistered;

    public static GlobalHotkeyRegistration TryRegister(
        nint hwnd,
        int id,
        SwitcherHotkeyModifier firstModifier,
        SwitcherHotkeyModifier secondModifier,
        SwitcherHotkeyKey key)
    {
        var registration = new GlobalHotkeyRegistration(hwnd, id);

        if (hwnd == 0)
        {
            registration.ErrorCode = -1;
            return registration;
        }

        var modifiers = ToNativeModifier(firstModifier) | ToNativeModifier(secondModifier);
        var virtualKey = ToVirtualKey(key);

        registration.isRegistered = RegisterHotKey(hwnd, id, modifiers, virtualKey);

        if (!registration.isRegistered)
        {
            registration.ErrorCode = Marshal.GetLastWin32Error();
        }

        return registration;
    }

    public void Dispose()
    {
        if (!isRegistered)
        {
            return;
        }

        _ = UnregisterHotKey(hwnd, id);
        isRegistered = false;
    }

    private static uint ToNativeModifier(SwitcherHotkeyModifier modifier) => modifier switch
    {
        SwitcherHotkeyModifier.Alt => 0x0001,
        SwitcherHotkeyModifier.Ctrl => 0x0002,
        SwitcherHotkeyModifier.Shift => 0x0004,
        _ => 0x0008
    };

    private static uint ToVirtualKey(SwitcherHotkeyKey key)
    {
        if (key is >= SwitcherHotkeyKey.A and <= SwitcherHotkeyKey.Z)
        {
            return (uint)('A' + key - SwitcherHotkeyKey.A);
        }

        if (key is >= SwitcherHotkeyKey.D0 and <= SwitcherHotkeyKey.D9)
        {
            return (uint)('0' + key - SwitcherHotkeyKey.D0);
        }

        if (key is >= SwitcherHotkeyKey.F1 and <= SwitcherHotkeyKey.F12)
        {
            return (uint)(0x70 + key - SwitcherHotkeyKey.F1);
        }

        return key switch
        {
            SwitcherHotkeyKey.Tab => 0x09,
            SwitcherHotkeyKey.Enter => 0x0D,
            _ => 0x20
        };
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);
}
