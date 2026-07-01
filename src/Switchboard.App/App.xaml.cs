using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Switchboard.App.ViewModels;
using Switchboard.Core.Services;

namespace Switchboard.App;

public partial class App : Application
{
    private ServiceProvider? serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWindowCatalog, DemoWindowCatalog>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<MainWindow>().Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
