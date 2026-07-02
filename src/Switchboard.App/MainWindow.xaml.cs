using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Switchboard.Core.Models;
using Switchboard.App.ViewModels;

namespace Switchboard.App;

public partial class MainWindow : Window
{
    private const double OuterMargin = 18;
    private const double ContentHorizontalMargin = 28;
    private const double ContentVerticalMargin = 20;
    private const double HeaderHeight = 52;
    private const double FooterHeight = 30;
    private const double GridCardWidth = 274;
    private const double GridCardHeight = 188;
    private const double CompactCardWidth = 274;
    private const double CompactCardHeight = 82;
    private const double ListWidth = 1120;
    private const double ListRowHeight = 54;
    private const double ItemHorizontalGap = 6;
    private const double ItemVerticalGap = 8;
    private const double SelectionFramePadding = 4;
    private const double LayoutSafetyPadding = 24;

    private readonly MainWindowViewModel viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        this.viewModel = viewModel;

        InitializeComponent();
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closed += OnClosed;
        PreviewKeyDown += OnPreviewKeyDown;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyContentSizedBounds();
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

    private void OnClosed(object? sender, EventArgs e) =>
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainWindowViewModel.VisibleWindows) or nameof(MainWindowViewModel.SelectedViewMode))
        {
            Dispatcher.BeginInvoke(ApplyContentSizedBounds, DispatcherPriority.Loaded);
        }
    }

    private void ApplyContentSizedBounds()
    {
        var windowCount = Math.Max(1, viewModel.VisibleWindows.Count);
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        var layout = CalculateLayout(windowCount, screenWidth, screenHeight);

        viewModel.SetGridColumnCount(layout.Columns);
        WindowState = WindowState.Normal;
        Width = layout.Width;
        Height = layout.Height;
        Left = Math.Max(0, (screenWidth - Width) / 2);
        Top = Math.Max(0, (screenHeight - Height) / 2);
    }

    private SwitcherLayout CalculateLayout(int windowCount, double screenWidth, double screenHeight)
    {
        var mode = viewModel.SelectedViewMode;
        var cardWidth = mode switch
        {
            SwitcherViewMode.Compact => CompactCardWidth,
            SwitcherViewMode.List => ListWidth,
            _ => GridCardWidth
        };
        var cardHeight = mode switch
        {
            SwitcherViewMode.Compact => CompactCardHeight,
            SwitcherViewMode.List => ListRowHeight,
            _ => GridCardHeight
        };

        var itemWidth = cardWidth + ItemHorizontalGap + SelectionFramePadding;
        var maxColumns = mode == SwitcherViewMode.List
            ? 1
            : Math.Max(1, (int)Math.Floor((screenWidth - (OuterMargin * 2) - ContentHorizontalMargin - LayoutSafetyPadding) / itemWidth));
        var preferredColumns = mode == SwitcherViewMode.List
            ? 1
            : GetPreferredGridColumns(windowCount);
        var columns = Math.Clamp(preferredColumns, 1, Math.Min(windowCount, maxColumns));

        while (columns < Math.Min(windowCount, maxColumns) &&
               CalculateDesiredHeight(windowCount, columns, cardHeight) > screenHeight)
        {
            columns++;
        }

        var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
        var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight);

        return new SwitcherLayout(
            columns,
            Math.Min(screenWidth, Math.Max(MinWidth, desiredWidth)),
            Math.Min(screenHeight, Math.Max(MinHeight, desiredHeight)));
    }

    private static int GetPreferredGridColumns(int windowCount) => windowCount switch
    {
        <= 3 => windowCount,
        <= 6 => 3,
        <= 12 => 4,
        <= 20 => 5,
        _ => (int)Math.Ceiling(Math.Sqrt(windowCount * 1.35))
    };

    private static double CalculateDesiredHeight(int windowCount, int columns, double cardHeight)
    {
        var rows = (int)Math.Ceiling(windowCount / (double)columns);
        return (OuterMargin * 2) +
            HeaderHeight +
            FooterHeight +
            ContentVerticalMargin +
            (rows * (cardHeight + ItemVerticalGap + SelectionFramePadding)) +
            LayoutSafetyPadding;
    }

    private readonly record struct SwitcherLayout(int Columns, double Width, double Height);

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
