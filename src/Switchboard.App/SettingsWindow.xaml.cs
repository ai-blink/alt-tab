using System.Windows;
using Switchboard.App.Localization;
using Switchboard.App.ViewModels;
using Switchboard.Core.Models;
using Switchboard.Native;

namespace Switchboard.App;

public partial class SettingsWindow : Window
{
    public SettingsWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => RefreshElevationState();
    }

    public event Action<OverlayAnchor>? RemoteMoveRequested;

    public event Action? ReturnToSavedPositionRequested;

    private void OnRemoteMoveClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string text } &&
            Enum.TryParse<OverlayAnchor>(text, out var anchor))
        {
            RemoteMoveRequested?.Invoke(anchor);
        }
    }

    private void OnReturnSavedClick(object sender, RoutedEventArgs e) =>
        ReturnToSavedPositionRequested?.Invoke();

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    // Task Scheduler is the source of truth for this option, so it is read back instead of persisted.
    private async void OnElevatedAutoStartClick(object sender, RoutedEventArgs e)
    {
        var enable = ElevatedAutoStartCheckBox.IsChecked == true;
        ElevatedAutoStartCheckBox.IsEnabled = false;

        var result = await Task.Run(() => enable
            ? ElevatedLogonTask.Register(Environment.ProcessPath!)
            : ElevatedLogonTask.Unregister());

        if (result == ElevatedLogonTaskResult.Failed)
        {
            ShowElevatedChangeFailed();
        }

        ElevatedAutoStartCheckBox.IsEnabled = true;
        RefreshElevationState();
    }

    private void OnRestartElevatedClick(object sender, RoutedEventArgs e)
    {
        if (System.Windows.Application.Current is App app && !app.RestartElevatedViaLogonTask())
        {
            ShowElevatedChangeFailed();
        }
    }

    private void ShowElevatedChangeFailed() =>
        System.Windows.MessageBox.Show(this, AppLocalizer.Get("Behavior.ElevatedChangeFailed"), Title, MessageBoxButton.OK, MessageBoxImage.Warning);

    private void RefreshElevationState()
    {
        var isRegistered = ElevatedLogonTask.IsRegistered();
        var isElevated = ElevatedLogonTask.IsCurrentProcessElevated();
        ElevatedAutoStartCheckBox.IsChecked = isRegistered;
        ElevationStatusText.SetResourceReference(
            System.Windows.Controls.TextBlock.TextProperty,
            isElevated ? "Behavior.RunningElevated" : "Behavior.RunningNormal");
        RestartElevatedButton.Visibility = isRegistered && !isElevated ? Visibility.Visible : Visibility.Collapsed;
    }
}
