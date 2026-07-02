using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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
    private const double ListDetailsHeaderHeight = 30;
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
        Closing += OnClosing;
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

    public void ShowOverlay()
    {
        ApplyContentSizedBounds();
        ShowInTaskbar = true;
        Show();
        Activate();
        Focus();
        WindowList.Focus();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (System.Windows.Application.Current is App { IsExitRequested: true })
        {
            return;
        }

        e.Cancel = true;
        ShowInTaskbar = false;
        Hide();
    }

    private void OnClosed(object? sender, EventArgs e) =>
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainWindowViewModel.VisibleWindows) or
            nameof(MainWindowViewModel.SelectedViewMode) or
            nameof(MainWindowViewModel.SelectedThumbnailScalePreset) or
            nameof(MainWindowViewModel.SelectedSizingPolicy))
        {
            Dispatcher.BeginInvoke(ApplyContentSizedBounds, DispatcherPriority.Loaded);
        }

        if (e.PropertyName == nameof(MainWindowViewModel.IsAlwaysOnTop))
        {
            Topmost = viewModel.IsAlwaysOnTop;
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
            SwitcherViewMode.Compact => viewModel.CompactCardWidth,
            SwitcherViewMode.List => viewModel.ListWidth,
            _ => viewModel.GridCardWidth
        };
        var cardHeight = mode switch
        {
            SwitcherViewMode.Compact => viewModel.CompactCardHeight,
            SwitcherViewMode.List => viewModel.ListRowHeight,
            _ => viewModel.GridCardHeight
        };
        var detailsHeaderHeight = mode == SwitcherViewMode.List ? ListDetailsHeaderHeight : 0;

        var itemWidth = cardWidth + ItemHorizontalGap + SelectionFramePadding;
        var availableColumns = Math.Max(1, (int)Math.Floor((screenWidth - (OuterMargin * 2) - ContentHorizontalMargin - LayoutSafetyPadding) / itemWidth));
        var maxColumns = mode == SwitcherViewMode.List
            ? Math.Min(2, availableColumns)
            : availableColumns;
        var preferredColumns = mode == SwitcherViewMode.List
            ? 2
            : viewModel.SelectedSizingPolicy == SwitcherSizingPolicy.Dense
                ? maxColumns
                : GetPreferredGridColumns(windowCount);
        var columns = Math.Clamp(preferredColumns, 1, Math.Min(windowCount, maxColumns));

        while (columns < Math.Min(windowCount, maxColumns) &&
               CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight) > screenHeight)
        {
            columns++;
        }

        var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
        var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight);

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

    private static double CalculateDesiredHeight(int windowCount, int columns, double cardHeight, double detailsHeaderHeight)
    {
        var rows = (int)Math.Ceiling(windowCount / (double)columns);
        return (OuterMargin * 2) +
            HeaderHeight +
            FooterHeight +
            ContentVerticalMargin +
            detailsHeaderHeight +
            (rows * (cardHeight + ItemVerticalGap + SelectionFramePadding)) +
            LayoutSafetyPadding;
    }

    private readonly record struct SwitcherLayout(int Columns, double Width, double Height);

    private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        if (e.OriginalSource is DependencyObject source && IsInteractiveHeaderElement(source))
        {
            return;
        }

        DragMove();
    }

    private static bool IsInteractiveHeaderElement(DependencyObject source) =>
        FindAncestor<System.Windows.Controls.Primitives.ButtonBase>(source) is not null ||
        FindAncestor<System.Windows.Controls.Primitives.TextBoxBase>(source) is not null;

    private static T? FindAncestor<T>(DependencyObject? source)
        where T : DependencyObject
    {
        while (source is not null)
        {
            if (source is T match)
            {
                return match;
            }

            source = VisualTreeHelper.GetParent(source) ?? LogicalTreeHelper.GetParent(source);
        }

        return null;
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
