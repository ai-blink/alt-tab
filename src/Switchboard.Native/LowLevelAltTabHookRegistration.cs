using System.Runtime.InteropServices;

namespace Switchboard.Native;

public sealed class LowLevelAltTabHookRegistration : IDisposable
{
    private const int WhKeyboardLl = 13;
    private const int WmKeydown = 0x0100;
    private const int WmKeyup = 0x0101;
    private const int WmSyskeydown = 0x0104;
    private const int WmSyskeyup = 0x0105;
    private const int WmQuit = 0x0012;
    private const int VkTab = 0x09;
    private const int VkMenu = 0x12;
    private const int LlkHfAltdown = 0x20;

    private readonly Action onAltTab;
    private readonly AltTabKeyFilter keyFilter = new();
    private readonly LowLevelKeyboardProc callback;
    private readonly ManualResetEventSlim registrationCompleted = new(false);
    private readonly Thread hookThread;
    private nint hookHandle;
    private uint hookThreadId;
    private bool isDisposed;

    private LowLevelAltTabHookRegistration(Action onAltTab)
    {
        this.onAltTab = onAltTab;
        callback = HookCallback;
        hookThread = new Thread(RunHookMessageLoop)
        {
            IsBackground = true,
            Name = "Switchboard Alt+Tab hook"
        };
    }

    public int ErrorCode { get; private set; }

    public bool IsRegistered => Volatile.Read(ref hookHandle) != 0;

    public static LowLevelAltTabHookRegistration TryRegister(Action onAltTab)
    {
        ArgumentNullException.ThrowIfNull(onAltTab);

        var registration = new LowLevelAltTabHookRegistration(onAltTab);
        registration.hookThread.Start();

        if (!registration.registrationCompleted.Wait(TimeSpan.FromSeconds(2)))
        {
            registration.ErrorCode = -1;
        }

        return registration;
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
        var threadId = Volatile.Read(ref hookThreadId);

        if (threadId != 0)
        {
            _ = PostThreadMessage(threadId, WmQuit, 0, 0);
        }

        if (hookThread.IsAlive && Thread.CurrentThread != hookThread)
        {
            _ = hookThread.Join(TimeSpan.FromSeconds(2));
        }

    }

    private void RunHookMessageLoop()
    {
        Volatile.Write(ref hookThreadId, GetCurrentThreadId());
        var moduleHandle = GetModuleHandle(null);
        var registeredHook = SetWindowsHookEx(WhKeyboardLl, callback, moduleHandle, 0);
        Volatile.Write(ref hookHandle, registeredHook);

        if (registeredHook == 0)
        {
            ErrorCode = Marshal.GetLastWin32Error();
            registrationCompleted.Set();
            return;
        }

        registrationCompleted.Set();

        try
        {
            while (GetMessage(out var message, 0, 0, 0) > 0)
            {
                _ = TranslateMessage(ref message);
                _ = DispatchMessage(ref message);
            }
        }
        finally
        {
            var handle = Interlocked.Exchange(ref hookHandle, 0);

            if (handle != 0)
            {
                _ = UnhookWindowsHookEx(handle);
            }
        }
    }

    private nint HookCallback(int code, nint wParam, nint lParam)
    {
        if (code >= 0)
        {
            var action = ClassifyAltTabKey(wParam, lParam);

            if (action == AltTabKeyAction.ToggleAndSuppress)
            {
                onAltTab();
            }

            if (action != AltTabKeyAction.PassThrough)
            {
                return 1;
            }
        }

        return CallNextHookEx(Volatile.Read(ref hookHandle), code, wParam, lParam);
    }

    private AltTabKeyAction ClassifyAltTabKey(nint wParam, nint lParam)
    {
        var info = Marshal.PtrToStructure<KeyboardHookInfo>(lParam);

        if (info.VirtualKeyCode != VkTab)
        {
            return AltTabKeyAction.PassThrough;
        }

        var isTabKeyDown = wParam is WmKeydown or WmSyskeydown;
        var isTabKeyUp = wParam is WmKeyup or WmSyskeyup;
        var isAltDown = (info.Flags & LlkHfAltdown) != 0 || IsKeyDown(VkMenu);
        return keyFilter.Process(isTabKeyDown, isTabKeyUp, isAltDown);
    }

    private static bool IsKeyDown(int virtualKey) => (GetAsyncKeyState(virtualKey) & 0x8000) != 0;

    private delegate nint LowLevelKeyboardProc(int code, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct KeyboardHookInfo
    {
        public readonly int VirtualKeyCode;
        public readonly int ScanCode;
        public readonly int Flags;
        public readonly int Time;
        public readonly nint ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public nint Hwnd;
        public uint Message;
        public nuint WParam;
        public nint LParam;
        public uint Time;
        public int PointX;
        public int PointY;
        public uint Private;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    private static extern int GetMessage(out NativeMessage lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref NativeMessage lpMsg);

    [DllImport("user32.dll")]
    private static extern nint DispatchMessage(ref NativeMessage lpMsg);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}
