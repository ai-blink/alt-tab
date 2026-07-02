using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Switchboard.App.ViewModels;
using Switchboard.Core.Services;
using Switchboard.Native;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace Switchboard.App;

public partial class App : System.Windows.Application
{
    private ServiceProvider? serviceProvider;
    private Forms.NotifyIcon? notifyIcon;

    public bool IsExitRequested { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        EnsureWindowsEnvironmentVariables();

        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<Win32NativeWindowProvider>();
        services.AddSingleton<IWindowCatalog>(provider => provider.GetRequiredService<Win32NativeWindowProvider>());
        services.AddSingleton<IWindowActivator>(provider => provider.GetRequiredService<Win32NativeWindowProvider>());
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        serviceProvider = services.BuildServiceProvider();

        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        InitializeTrayIcon();

        MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        ShowOverlay();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        notifyIcon?.Dispose();
        serviceProvider?.Dispose();
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
        notifyIcon?.Dispose();
        notifyIcon = null;
        Shutdown();
    }

    private void InitializeTrayIcon()
    {
        var openItem = new Forms.ToolStripMenuItem("Open Switchboard", null, (_, _) => Dispatcher.Invoke(ShowOverlay));
        var exitItem = new Forms.ToolStripMenuItem("Exit", null, (_, _) => Dispatcher.Invoke(ExitFromTray));

        notifyIcon = new Forms.NotifyIcon
        {
            Icon = Drawing.SystemIcons.Application,
            Text = "Switchboard",
            Visible = true,
            ContextMenuStrip = new Forms.ContextMenuStrip()
        };

        notifyIcon.ContextMenuStrip.Items.Add(openItem);
        notifyIcon.ContextMenuStrip.Items.Add(new Forms.ToolStripSeparator());
        notifyIcon.ContextMenuStrip.Items.Add(exitItem);
        notifyIcon.DoubleClick += (_, _) => Dispatcher.Invoke(ShowOverlay);
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
