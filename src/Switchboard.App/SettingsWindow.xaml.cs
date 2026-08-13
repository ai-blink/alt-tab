using System.Windows;
using Switchboard.App.ViewModels;
using Switchboard.Core.Models;

namespace Switchboard.App;

public partial class SettingsWindow : Window
{
    public SettingsWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
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
}
