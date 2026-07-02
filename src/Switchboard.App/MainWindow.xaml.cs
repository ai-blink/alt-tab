using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Switchboard.App.ViewModels;

namespace Switchboard.App;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel;

        InitializeComponent();
        DataContext = viewModel;

        Loaded += OnLoaded;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Normal;
        ShowInTaskbar = true;

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                Activate();
                Focus();
                WindowList.Focus();
            }),
            DispatcherPriority.ApplicationIdle);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Close();
                e.Handled = true;
                return;

            case Key.Tab:
                MoveSelection((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                    ? viewModel.SelectPreviousWindow
                    : viewModel.SelectNextWindow);
                e.Handled = true;
                return;

            case Key.Left:
                MoveSelection(viewModel.SelectPreviousWindow);
                e.Handled = true;
                return;

            case Key.Right:
                MoveSelection(viewModel.SelectNextWindow);
                e.Handled = true;
                return;

            case Key.Up:
                MoveSelection(viewModel.SelectWindowAbove);
                e.Handled = true;
                return;

            case Key.Down:
                MoveSelection(viewModel.SelectWindowBelow);
                e.Handled = true;
                return;

            case Key.Return:
                ActivateSelectedWindowAndClose();
                e.Handled = true;
                return;
        }
    }

    private void MoveSelection(Action select)
    {
        select();

        if (viewModel.SelectedWindow is not null)
        {
            WindowList.ScrollIntoView(viewModel.SelectedWindow);
        }
    }

    private void ActivateSelectedWindowAndClose()
    {
        if (viewModel.TryActivateSelectedWindow())
        {
            Close();
        }
    }

    private void OnWindowListMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left ||
            ItemsControl.ContainerFromElement(WindowList, e.OriginalSource as DependencyObject) is not ListBoxItem item)
        {
            return;
        }

        WindowList.SelectedItem = item.DataContext;
        ActivateSelectedWindowAndClose();
        e.Handled = true;
    }

    private void OnChromeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }

        DragMove();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
