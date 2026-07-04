using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Switchboard.Core.Models;
using Switchboard.App.ViewModels;
using Switchboard.Native;

namespace Switchboard.App;

public partial class MainWindow : Window
{
    private const int WmHotkey = 0x0312;
    private const int SwitchboardHotkeyId = 0x5342;
    private const double OuterMargin = 18;
    private const double ContentHorizontalMargin = 28;
    private const double ContentVerticalMargin = 20;
    private const double HeaderHeight = 52;
    private const double FooterHeight = 30;
    private const double ListDetailsHeaderHeight = 30;
    private const double ItemHorizontalGap = 6;
    private const double ItemVerticalGap = 8;
    private const double SelectionFramePadding = 8;
    private const double LayoutSafetyPadding = 24;

    private readonly MainWindowViewModel viewModel;
    private HwndSource? hwndSource;
    private GlobalHotkeyRegistration? hotkeyRegistration;

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
        InitializeGlobalHotkey();

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

    private void OnClosed(object? sender, EventArgs e)
    {
        hotkeyRegistration?.Dispose();
        hotkeyRegistration = null;
        hwndSource?.RemoveHook(WndProc);
        hwndSource = null;
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

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

        if (e.PropertyName is nameof(MainWindowViewModel.SelectedFirstHotkeyModifier) or
            nameof(MainWindowViewModel.SelectedSecondHotkeyModifier) or
            nameof(MainWindowViewModel.SelectedHotkeyKey))
        {
            RegisterGlobalHotkey();
        }
    }

    private void InitializeGlobalHotkey()
    {
        if (hwndSource is not null)
        {
            return;
        }

        hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        hwndSource?.AddHook(WndProc);
        RegisterGlobalHotkey();
    }

    private void RegisterGlobalHotkey()
    {
        hotkeyRegistration?.Dispose();
        hotkeyRegistration = null;

        var hwnd = new WindowInteropHelper(this).Handle;

        if (hwnd == 0)
        {
            return;
        }

        hotkeyRegistration = GlobalHotkeyRegistration.TryRegister(
            hwnd,
            SwitchboardHotkeyId,
            viewModel.SelectedFirstHotkeyModifier,
            viewModel.SelectedSecondHotkeyModifier,
            viewModel.SelectedHotkeyKey);
    }

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == WmHotkey && wParam.ToInt32() == SwitchboardHotkeyId)
        {
            ShowOverlay();
            handled = true;
        }

        return 0;
    }

    private void ApplyContentSizedBounds()
    {
        var windowCount = Math.Max(1, viewModel.VisibleWindows.Count);
        var workArea = SystemParameters.WorkArea;
        var screenWidth = workArea.Width;
        var screenHeight = workArea.Height;
        var layout = CalculateLayout(windowCount, screenWidth, screenHeight);

        viewModel.SetGridColumnCount(layout.Columns);
        WindowState = WindowState.Normal;
        Width = layout.Width;
        Height = layout.Height;
        Left = workArea.Left + Math.Max(0, (screenWidth - Width) / 2);
        Top = workArea.Top + Math.Max(0, (screenHeight - Height) / 2);
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
        var columns = mode == SwitcherViewMode.List
            ? Math.Clamp(2, 1, Math.Min(windowCount, maxColumns))
            : ChooseBestColumns(windowCount, maxColumns, itemWidth, cardHeight, detailsHeaderHeight, screenWidth, screenHeight);

        var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
        var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight);

        return new SwitcherLayout(
            columns,
            Math.Min(screenWidth, Math.Max(MinWidth, desiredWidth)),
            Math.Min(screenHeight, Math.Max(MinHeight, desiredHeight)));
    }

    private int ChooseBestColumns(
        int windowCount,
        int maxColumns,
        double itemWidth,
        double cardHeight,
        double detailsHeaderHeight,
        double screenWidth,
        double screenHeight)
    {
        var columnLimit = Math.Max(1, Math.Min(windowCount, maxColumns));
        var best = new LayoutCandidate(1, double.MaxValue);

        for (var columns = 1; columns <= columnLimit; columns++)
        {
            var rows = (int)Math.Ceiling(windowCount / (double)columns);
            var emptySlots = (rows * columns) - windowCount;
            var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
            var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight);
            var widthOverflow = Math.Max(0, desiredWidth - screenWidth);
            var heightOverflow = Math.Max(0, desiredHeight - screenHeight);
            var rightBlankRatio = Math.Max(0, screenWidth - desiredWidth) / screenWidth;
            var rowColumnBalance = Math.Abs(rows - columns);
            var denseWeight = viewModel.SelectedSizingPolicy == SwitcherSizingPolicy.Dense ? 1.0 : 0.0;

            var score =
                (emptySlots * (55 - (denseWeight * 25))) +
                (rightBlankRatio * (70 + (denseWeight * 45))) +
                (rowColumnBalance * (3 + (denseWeight * 2))) +
                (rows * 1.8) +
                (widthOverflow * 100) +
                (heightOverflow * 100);

            if (score < best.Score)
            {
                best = new LayoutCandidate(columns, score);
            }
        }

        return best.Columns;
    }

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

    private readonly record struct LayoutCandidate(int Columns, double Score);

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
