using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Switchboard.Native;

// Diagnostics only: reports when the Windows shell switcher opens instead of Switchboard,
// together with the window that was in front, so misses can be attributed to an elevated
// foreground window (UIPI hides it from the low-level hook) or to a hook timeout.
// Out-of-context WinEvents are delivered through the registering thread's message loop.
public sealed class AltTabMissWatcher : IDisposable
{
    private const uint EventSystemForeground = 0x0003;
    private const uint EventSystemSwitchStart = 0x0014;
    private const uint WinEventOutOfContext = 0x0000;
    private const uint WinEventSkipOwnProcess = 0x0002;
    private const int VkMenu = 0x12;
    private const string ShellSwitcherClass = "XamlExplorerHostIslandWindow";

    // Transient shell windows that take the foreground during a switch; they are never the "previous" window.
    private static readonly HashSet<string> TransientShellClasses =
        [ShellSwitcherClass, "ForegroundStaging", "MultitaskingViewFrame", "TaskSwitcherWnd"];
    private static readonly TimeSpan DuplicateWindow = TimeSpan.FromSeconds(1);

    private readonly Action<string> onMiss;
    private readonly WinEventProc callback;
    private readonly nint[] hooks;
    private nint lastForegroundWindow;
    private long lastReportTimestamp;

    private AltTabMissWatcher(Action<string> onMiss)
    {
        this.onMiss = onMiss;
        callback = OnWinEvent;
        hooks =
        [
            SetWinEventHook(EventSystemForeground, EventSystemForeground, 0, callback, 0, 0, WinEventOutOfContext | WinEventSkipOwnProcess),
            SetWinEventHook(EventSystemSwitchStart, EventSystemSwitchStart, 0, callback, 0, 0, WinEventOutOfContext | WinEventSkipOwnProcess)
        ];
    }

    public static AltTabMissWatcher Start(Action<string> onMiss)
    {
        ArgumentNullException.ThrowIfNull(onMiss);
        return new AltTabMissWatcher(onMiss);
    }

    public void Dispose()
    {
        foreach (var hook in hooks)
        {
            if (hook != 0)
            {
                _ = UnhookWinEvent(hook);
            }
        }
    }

    private void OnWinEvent(nint hook, uint eventType, nint hwnd, int objectId, int childId, uint thread, uint time)
    {
        if (eventType == EventSystemSwitchStart)
        {
            Report("switch-start");
            return;
        }

        var className = GetClass(hwnd);

        if (className == ShellSwitcherClass && (GetAsyncKeyState(VkMenu) & 0x8000) != 0)
        {
            Report("shell-switcher-foreground");
            return;
        }

        if (!TransientShellClasses.Contains(className))
        {
            lastForegroundWindow = hwnd;
        }
    }

    private void Report(string signal)
    {
        var now = Stopwatch.GetTimestamp();

        if (Stopwatch.GetElapsedTime(lastReportTimestamp, now) < DuplicateWindow)
        {
            return;
        }

        lastReportTimestamp = now;
        onMiss($"{signal}; previous foreground {DescribeWindow(lastForegroundWindow)}");
    }

    private static string DescribeWindow(nint hwnd)
    {
        if (hwnd == 0)
        {
            return "unknown";
        }

        _ = GetWindowThreadProcessId(hwnd, out var processId);
        string processName;

        try
        {
            using var process = Process.GetProcessById((int)processId);
            processName = process.ProcessName;
        }
        catch (ArgumentException)
        {
            processName = "exited";
        }

        return $"{processName} (pid {processId}, class {GetClass(hwnd)}, elevation {DescribeElevation(processId)})";
    }

    private static string DescribeElevation(uint processId)
    {
        var process = OpenProcess(ProcessQueryLimitedInformation, false, processId);

        if (process == 0)
        {
            return "unknown";
        }

        try
        {
            if (!OpenProcessToken(process, TokenQuery, out var token))
            {
                // A medium-integrity process cannot open an elevated process token.
                return "likely-elevated";
            }

            try
            {
                return GetTokenInformation(token, TokenElevation, out var elevated, sizeof(int), out _) && elevated != 0
                    ? "elevated"
                    : "not-elevated";
            }
            finally
            {
                _ = CloseHandle(token);
            }
        }
        finally
        {
            _ = CloseHandle(process);
        }
    }

    private static string GetClass(nint hwnd)
    {
        var builder = new StringBuilder(128);
        return GetClassName(hwnd, builder, builder.Capacity) > 0 ? builder.ToString() : string.Empty;
    }

    private const uint ProcessQueryLimitedInformation = 0x1000;
    private const uint TokenQuery = 0x0008;
    private const int TokenElevation = 20;

    private delegate void WinEventProc(nint hook, uint eventType, nint hwnd, int objectId, int childId, uint thread, uint time);

    [DllImport("user32.dll")]
    private static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint module, WinEventProc callback, uint processId, uint threadId, uint flags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(nint hook);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(nint hwnd, StringBuilder className, int maxCount);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hwnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int virtualKey);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(uint access, bool inheritHandle, uint processId);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(nint process, uint access, out nint token);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool GetTokenInformation(nint token, int informationClass, out int information, int length, out int returnLength);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint handle);
}
