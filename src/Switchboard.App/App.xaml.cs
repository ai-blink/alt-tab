using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Switchboard.App.ViewModels;
using Switchboard.Core.Services;
using Switchboard.Native;

namespace Switchboard.App;

public partial class App : Application
{
    private ServiceProvider? serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        EnsureWindowsEnvironmentVariables();

        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<IWindowCatalog, Win32NativeWindowProvider>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        serviceProvider = services.BuildServiceProvider();

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        serviceProvider?.Dispose();
        base.OnExit(e);
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
