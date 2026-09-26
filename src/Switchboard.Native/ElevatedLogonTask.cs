using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace Switchboard.Native;

public enum ElevatedLogonTaskResult
{
    Succeeded,
    Cancelled,
    Failed
}

// A standard-rights process never sees keyboard input aimed at an elevated foreground window (UIPI),
// so Alt+Tab over Task Manager and similar windows falls through to the Windows switcher. This task
// starts Switchboard with the user's highest privileges at sign-in; Task Scheduler launches it
// without a UAC prompt, and only creating or deleting the task needs elevation.
[SupportedOSPlatform("windows")]
public static class ElevatedLogonTask
{
    // ASCII only: schtasks echoes names and XML through the 8-bit console code page.
    public const string TaskName = "Switchboard-AutoStart";

    private const int ErrorCancelled = 1223;

    public static bool IsCurrentProcessElevated()
    {
        using var identity = WindowsIdentity.GetCurrent();
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static bool IsRegistered() => RunSchtasks($"/Query /TN \"{TaskName}\"", elevate: false) == 0;

    public static bool RunNow() => RunSchtasks($"/Run /TN \"{TaskName}\"", elevate: false) == 0;

    public static ElevatedLogonTaskResult Register(string executablePath)
    {
        using var identity = WindowsIdentity.GetCurrent();
        var xmlPath = Path.Combine(Path.GetTempPath(), $"switchboard-task-{Guid.NewGuid():N}.xml");

        try
        {
            // schtasks reads task XML as UTF-16 LE with a BOM.
            File.WriteAllText(xmlPath, BuildTaskXml(executablePath, identity.Name), new UnicodeEncoding(bigEndian: false, byteOrderMark: true));
            return RunElevated($"/Create /TN \"{TaskName}\" /XML \"{xmlPath}\" /F");
        }
        finally
        {
            File.Delete(xmlPath);
        }
    }

    public static ElevatedLogonTaskResult Unregister() => RunElevated($"/Delete /TN \"{TaskName}\" /F");

    // Task schema 1.2: 1.3-only elements are rejected and <Settings> children must keep this order.
    // Priority 4 is normal process priority; the scheduler default (7) is below normal and would slow the hook.
    public static string BuildTaskXml(string executablePath, string userId)
    {
        var command = SecurityElement.Escape(executablePath);
        var workingDirectory = SecurityElement.Escape(Path.GetDirectoryName(executablePath) ?? string.Empty);
        var user = SecurityElement.Escape(userId);

        return $"""
            <?xml version="1.0" encoding="UTF-16"?>
            <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
              <RegistrationInfo>
                <Description>Starts Switchboard with highest privileges at sign-in so Alt+Tab also works over administrator windows.</Description>
              </RegistrationInfo>
              <Triggers>
                <LogonTrigger>
                  <Enabled>true</Enabled>
                  <UserId>{user}</UserId>
                </LogonTrigger>
              </Triggers>
              <Principals>
                <Principal id="Author">
                  <UserId>{user}</UserId>
                  <LogonType>InteractiveToken</LogonType>
                  <RunLevel>HighestAvailable</RunLevel>
                </Principal>
              </Principals>
              <Settings>
                <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
                <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
                <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
                <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
                <Priority>4</Priority>
              </Settings>
              <Actions Context="Author">
                <Exec>
                  <Command>{command}</Command>
                  <WorkingDirectory>{workingDirectory}</WorkingDirectory>
                </Exec>
              </Actions>
            </Task>
            """;
    }

    private static ElevatedLogonTaskResult RunElevated(string arguments)
    {
        try
        {
            return RunSchtasks(arguments, elevate: true) == 0
                ? ElevatedLogonTaskResult.Succeeded
                : ElevatedLogonTaskResult.Failed;
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode == ErrorCancelled)
        {
            return ElevatedLogonTaskResult.Cancelled;
        }
    }

    private static int RunSchtasks(string arguments, bool elevate)
    {
        var startInfo = new ProcessStartInfo("schtasks.exe", arguments)
        {
            UseShellExecute = elevate,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        if (elevate)
        {
            startInfo.Verb = "runas";
        }
        else
        {
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
        }

        using var process = Process.Start(startInfo)!;

        if (!elevate)
        {
            _ = process.StandardOutput.ReadToEnd();
            _ = process.StandardError.ReadToEnd();
        }

        process.WaitForExit();
        return process.ExitCode;
    }
}
