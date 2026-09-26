using System.IO;
using System.Runtime;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Switchboard.App.Localization;
using Switchboard.App.ViewModels;
using Switchboard.Core.Services;
using Switchboard.Native;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace Switchboard.App;

public partial class App : System.Windows.Application
{
    // One Switchboard per session: two instances would both toggle on the same Alt+Tab and cancel out.
    // Opening the mutex of an elevated instance from a standard one throws UnauthorizedAccessException.
    private const string SingleInstanceMutexName = @"Local\Switchboard.App.SingleInstance";
    private Mutex? singleInstanceMutex;
    private ServiceProvider? serviceProvider;
    private Forms.NotifyIcon? notifyIcon;
    private Drawing.Icon? trayIcon;

    public bool IsExitRequested { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!TryAcquireSingleInstance())
        {
            Shutdown();
            return;
        }

        EnsureWindowsEnvironmentVariables();

        // The Alt+Tab hook must answer within LowLevelHooksTimeout; long blocking GCs or EcoQoS
        // throttling would hand that key press to the Windows switcher instead.
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        _ = ProcessResponsiveness.DisableExecutionSpeedThrottling();

        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<IWorkAreaProvider, Win32WorkAreaProvider>();
        services.AddSingleton<Win32NativeWindowProvider>();
        services.AddSingleton<IWindowCatalog>(provider => provider.GetRequiredService<Win32NativeWindowProvider>());
        services.AddSingleton<IWindowActivator>(provider => provider.GetRequiredService<Win32NativeWindowProvider>());
        services.AddSingleton<IWindowCloser>(provider => provider.GetRequiredService<Win32NativeWindowProvider>());
        services.AddSingleton<IUserSettingsStore, JsonUserSettingsStore>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        serviceProvider = services.BuildServiceProvider();
        AppLocalizer.Apply(serviceProvider.GetRequiredService<MainWindowViewModel>().SelectedLanguage);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        InitializeTrayIcon();

        MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        ShowOverlay();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        DisposeTrayIcon();
        serviceProvider?.Dispose();
        ReleaseSingleInstance();
        base.OnExit(e);
    }

    public void ShowOverlay()
    {
        if (MainWindow is MainWindow mainWindow)
        {
            mainWindow.ShowOverlay();
        }
    }

    public void ExitFromTray()
    {
        IsExitRequested = true;
        DisposeTrayIcon();
        Shutdown();
    }

    // Hands over to the elevated logon task: the mutex is released first so the new instance can start.
    public bool RestartElevatedViaLogonTask()
    {
        ReleaseSingleInstance();

        if (!ElevatedLogonTask.RunNow())
        {
            _ = TryAcquireSingleInstance();
            return false;
        }

        ExitFromTray();
        return true;
    }

    private bool TryAcquireSingleInstance()
    {
        try
        {
            singleInstanceMutex = new Mutex(initiallyOwned: true, SingleInstanceMutexName, out var createdNew);

            if (createdNew)
            {
                return true;
            }

            singleInstanceMutex.Dispose();
            singleInstanceMutex = null;
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private void ReleaseSingleInstance()
    {
        if (singleInstanceMutex is null)
        {
            return;
        }

        singleInstanceMutex.ReleaseMutex();
        singleInstanceMutex.Dispose();
        singleInstanceMutex = null;
    }

    private void InitializeTrayIcon()
    {
        var openItem = new Forms.ToolStripMenuItem("Open Switchboard", null, (_, _) => Dispatcher.Invoke(ShowOverlay));
        var exitItem = new Forms.ToolStripMenuItem("Exit", null, (_, _) => Dispatcher.Invoke(ExitFromTray));

        notifyIcon = new Forms.NotifyIcon
        {
            Icon = trayIcon = SwitchboardIconFactory.CreateTrayIcon(),
            Text = "Switchboard",
            Visible = true,
            ContextMenuStrip = new Forms.ContextMenuStrip()
        };

        notifyIcon.ContextMenuStrip.Items.Add(openItem);
        notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripSeparator());
        notifyIcon.ContextMenuStrip.Items.Add(exitItem);
        notifyIcon.DoubleClick += (_, _) => Dispatcher.Invoke(ShowOverlay);
    }

    private void DisposeTrayIcon()
    {
        notifyIcon?.Dispose();
        notifyIcon = null;
        trayIcon?.Dispose();
        trayIcon = null;
    }

    private static void EnsureWindowsEnvironmentVariables()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("windir")))
        {
            return;
        }

        var windowsDirectory = Environment.GetEnvironmentVariable("SystemRoot");

        if (string.IsNullOrWhiteSpace(windowsDirectory))
        {
            var systemDirectory = Environment.SystemDirectory;
            windowsDirectory = string.IsNullOrWhiteSpace(systemDirectory)
                ? null
                : Directory.GetParent(systemDirectory)?.FullName;
        }

        if (!string.IsNullOrWhiteSpace(windowsDirectory))
        {
            Environment.SetEnvironmentVariable("windir", windowsDirectory);
        }
    }
}
