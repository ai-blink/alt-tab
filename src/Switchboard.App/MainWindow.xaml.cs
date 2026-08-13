using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Switchboard.Core.Models;
using Switchboard.Core.Services;
using Switchboard.App.ViewModels;
using Switchboard.Native;

namespace Switchboard.App;

public partial class MainWindow : Window
{
    private const int WmHotkey = 0x0312;
    private const int SwitchboardHotkeyId = 0x5342;
    private const int AltTabHotkeyId = 0x5343;
    private const double CompactWorkAreaRatio = 0.65;
    private readonly MainWindowViewModel viewModel;
    private readonly IWorkAreaProvider workAreaProvider;
    private HwndSource? hwndSource;
    private GlobalHotkeyRegistration? hotkeyRegistration;
    private GlobalHotkeyRegistration? altTabHotkeyRegistration;
    private LowLevelAltTabHookRegistration? altTabHookRegistration;
    private readonly DispatcherTimer refreshTimer;
    private nint previousForegroundWindow;
    private double currentLayoutWidth = 955;
    private double currentLayoutHeight = 540;
    private SettingsWindow? settingsWindow;
    private OverlayPosition? temporaryOverlayPosition;
    private OverlayWorkArea? temporaryOverlayWorkArea;

    public MainWindow(MainWindowViewModel viewModel, IWorkAreaProvider workAreaProvider)
    {
        this.viewModel = viewModel;
        this.workAreaProvider = workAreaProvider;
        refreshTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        refreshTimer.Tick += OnRefreshTimerTick;

        InitializeComponent();
        Icon = SwitchboardIconFactory.CreateWindowIcon();
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        Closed += OnClosed;
        PreviewKeyDown += OnPreviewKeyDown;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RefreshWindowCatalog();
        ApplyContentSizedBounds();
        ShowInTaskbar = true;
        StartRefreshTimer();
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
        if (!IsVisible)
        {
            RememberForegroundWindow();
        }

        RefreshWindowCatalog();
        ApplyContentSizedBounds();
        ShowInTaskbar = true;
        Show();
        StartRefreshTimer();
        var hwnd = new WindowInteropHelper(this).Handle;
        _ = ForegroundWindowPresenter.TryPresent(hwnd, viewModel.IsAlwaysOnTop);
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
        HideOverlay(restorePreviousWindow: false);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        settingsWindow?.Close();
        settingsWindow = null;
        StopRefreshTimer();
        refreshTimer.Tick -= OnRefreshTimerTick;
        altTabHotkeyRegistration?.Dispose();
        altTabHotkeyRegistration = null;
        altTabHookRegistration?.Dispose();
        altTabHookRegistration = null;
        hotkeyRegistration?.Dispose();
        hotkeyRegistration = null;
        hwndSource?.RemoveHook(WndProc);
        hwndSource = null;
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnRefreshTimerTick(object? sender, EventArgs e)
    {
        if (!IsVisible)
        {
            StopRefreshTimer();
            return;
        }

        RefreshWindowCatalog();
    }

    private void RefreshWindowCatalog()
    {
        viewModel.RefreshWindows();
    }

    private void StartRefreshTimer()
    {
        if (!refreshTimer.IsEnabled)
        {
            refreshTimer.Start();
        }
    }

    private void StopRefreshTimer()
    {
        if (refreshTimer.IsEnabled)
        {
            refreshTimer.Stop();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainWindowViewModel.VisibleWindows) or
            nameof(MainWindowViewModel.SelectedViewMode) or
            nameof(MainWindowViewModel.SelectedThumbnailScalePreset) or
            nameof(MainWindowViewModel.SelectedSizingPolicy) or
            nameof(MainWindowViewModel.IsCompactOverlayEnabled) or
            nameof(MainWindowViewModel.SelectedCompactOverlayPlacement) or
            nameof(MainWindowViewModel.SavedOverlayPosition))
        {
            Dispatcher.BeginInvoke(ApplyContentSizedBounds, DispatcherPriority.Loaded);
        }

        if (e.PropertyName == nameof(MainWindowViewModel.SelectedOverlayScalePreset))
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
        RegisterAltTabHook();
        RegisterAltTabHotkeyFallback();
        RegisterGlobalHotkey();
    }

    private void RegisterAltTabHook()
    {
        altTabHookRegistration?.Dispose();
        altTabHookRegistration = null;
        altTabHookRegistration = LowLevelAltTabHookRegistration.TryRegister(() =>
            Dispatcher.BeginInvoke(ToggleOverlay, DispatcherPriority.Input));
    }

    private void RegisterAltTabHotkeyFallback()
    {
        altTabHotkeyRegistration?.Dispose();
        altTabHotkeyRegistration = null;

        if (altTabHookRegistration?.IsRegistered == true)
        {
            return;
        }

        var hwnd = new WindowInteropHelper(this).Handle;

        if (hwnd == 0)
        {
            return;
        }

        // Passing Alt twice intentionally produces a single MOD_ALT bit.
        altTabHotkeyRegistration = GlobalHotkeyRegistration.TryRegister(
            hwnd,
            AltTabHotkeyId,
            SwitcherHotkeyModifier.Alt,
            SwitcherHotkeyModifier.Alt,
            SwitcherHotkeyKey.Tab);
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
        if (msg != WmHotkey)
        {
            return 0;
        }

        if (wParam.ToInt32() == SwitchboardHotkeyId)
        {
            ShowOverlay();
            handled = true;
        }
        else if (wParam.ToInt32() == AltTabHotkeyId)
        {
            ToggleOverlay();
            handled = true;
        }

        return 0;
    }

    private void ToggleOverlay()
    {
        if (IsVisible)
        {
            HideOverlay(restorePreviousWindow: true);
            return;
        }

        ShowOverlay();
    }

    private void HideOverlay(bool restorePreviousWindow)
    {
        temporaryOverlayPosition = null;
        temporaryOverlayWorkArea = null;
        ShowInTaskbar = false;
        StopRefreshTimer();
        Hide();

        if (restorePreviousWindow)
        {
            var hwnd = previousForegroundWindow;
            previousForegroundWindow = 0;
            RestoreForegroundWindowAfterInput(hwnd);
        }
    }

    private void RestoreForegroundWindowAfterInput(nint hwnd)
    {
        if (hwnd == 0)
        {
            return;
        }

        var restoreTimer = new DispatcherTimer(DispatcherPriority.Input)
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        restoreTimer.Tick += (_, _) =>
        {
            restoreTimer.Stop();
            _ = ForegroundWindowPresenter.TryRestore(hwnd);
        };
        restoreTimer.Start();
    }

    private void RememberForegroundWindow()
    {
        var currentWindow = ForegroundWindowPresenter.GetCurrentWindow();
        var overlayWindow = new WindowInteropHelper(this).Handle;

        if (currentWindow != 0 && currentWindow != overlayWindow)
        {
            previousForegroundWindow = currentWindow;
        }
    }

    private void ApplyContentSizedBounds()
    {
        var windowCount = Math.Max(1, viewModel.VisibleWindows.Count);
        var workArea = ResolveSavedPositionWorkArea();
        var appScale = viewModel.PresentationScale;
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
        var minimumCompactWidth = SwitcherLayoutCalculator.ScaleLayoutDimension(
            SwitcherLayoutCalculator.MinimumLayoutWidth,
            appScale);
        var minimumCompactHeight = SwitcherLayoutCalculator.ScaleLayoutDimension(
            SwitcherLayoutCalculator.CalculateMinimumLayoutHeight(mode, cardHeight),
            appScale);
        var layoutWorkAreaWidth = viewModel.IsCompactOverlayEnabled
            ? Math.Min(workArea.Width, Math.Max(workArea.Width * CompactWorkAreaRatio, minimumCompactWidth))
            : workArea.Width;
        var layoutWorkAreaHeight = viewModel.IsCompactOverlayEnabled
            ? Math.Min(workArea.Height, Math.Max(workArea.Height * CompactWorkAreaRatio, minimumCompactHeight))
            : workArea.Height;
        var layout = SwitcherLayoutCalculator.Calculate(
            windowCount,
            layoutWorkAreaWidth,
            layoutWorkAreaHeight,
            appScale,
            mode,
            viewModel.SelectedSizingPolicy,
            cardWidth,
            cardHeight);

        viewModel.SetGridColumnCount(layout.Columns);
        currentLayoutWidth = layout.Width;
        currentLayoutHeight = layout.Height;
        ScrollViewer.SetVerticalScrollBarVisibility(
            WindowList,
            layout.RequiresVerticalScroll ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden);
        WindowState = WindowState.Normal;
        Width = Math.Min(
            layoutWorkAreaWidth,
            SwitcherLayoutCalculator.ScaleLayoutDimension(currentLayoutWidth, appScale));
        Height = Math.Min(
            layoutWorkAreaHeight,
            SwitcherLayoutCalculator.ScaleLayoutDimension(currentLayoutHeight, appScale));
        var position = temporaryOverlayPosition is { } temporaryPosition
            ? OverlayPositionCalculator.Constrain(
                temporaryOverlayWorkArea ?? workArea,
                temporaryPosition,
                Width,
                Height)
            : OverlayPositionCalculator.Calculate(
                workArea,
                Width,
                Height,
                viewModel.SavedOverlayPosition ?? new OverlayPositionPreference());
        Left = position.Left;
        Top = position.Top;
    }

    private OverlayWorkArea ResolveSavedPositionWorkArea()
    {
        var savedMonitor = viewModel.SavedOverlayPosition?.MonitorDeviceName;

        return workAreaProvider.TryGetWorkArea(savedMonitor, out var savedWorkArea)
            ? ToLogicalWorkArea(savedWorkArea)
            : GetCurrentWorkArea();
    }

    private OverlayWorkArea GetCurrentWorkArea()
    {
        var windowHandle = new WindowInteropHelper(this).Handle;
        var physicalWorkArea = windowHandle == 0
            ? workAreaProvider.GetPrimaryWorkArea()
            : workAreaProvider.GetWorkAreaForWindow(windowHandle);

        return ToLogicalWorkArea(physicalWorkArea);
    }

    private OverlayWorkArea ToLogicalWorkArea(OverlayWorkArea physicalWorkArea)
    {
        var source = PresentationSource.FromVisual(this) as HwndSource;
        var transform = source?.CompositionTarget is { } target
            ? target.TransformFromDevice
            : Matrix.Identity;
        var topLeft = transform.Transform(new System.Windows.Point(physicalWorkArea.Left, physicalWorkArea.Top));
        var bottomRight = transform.Transform(new System.Windows.Point(physicalWorkArea.Right, physicalWorkArea.Bottom));

        return new OverlayWorkArea(
            physicalWorkArea.DeviceName,
            topLeft.X,
            topLeft.Y,
            Math.Max(0, bottomRight.X - topLeft.X),
            Math.Max(0, bottomRight.Y - topLeft.Y));
    }

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
            e.OriginalSource is DependencyObject source &&
            FindAncestor<System.Windows.Controls.Primitives.ButtonBase>(source) is not null ||
            ItemsControl.ContainerFromElement(WindowList, e.OriginalSource as DependencyObject) is not ListBoxItem item)
        {
            return;
        }

        WindowList.SelectedItem = item.DataContext;
        ActivateSelectedWindowAndClose();
        e.Handled = true;
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
    {
        if (settingsWindow is { IsVisible: true } openSettingsWindow)
        {
            openSettingsWindow.Activate();
            return;
        }

        viewModel.SelectedSettingsTab = SettingsTab.Position;
        var newSettingsWindow = new SettingsWindow(viewModel)
        {
            Owner = this
        };
        newSettingsWindow.RemoteMoveRequested += OnRemoteMoveRequested;
        newSettingsWindow.ReturnToSavedPositionRequested += OnReturnToSavedPositionRequested;
        newSettingsWindow.Closed += OnSettingsWindowClosed;
        settingsWindow = newSettingsWindow;
        newSettingsWindow.ShowDialog();
    }

    private void OnSettingsWindowClosed(object? sender, EventArgs e)
    {
        if (sender is not SettingsWindow closedSettingsWindow)
        {
            return;
        }

        closedSettingsWindow.RemoteMoveRequested -= OnRemoteMoveRequested;
        closedSettingsWindow.ReturnToSavedPositionRequested -= OnReturnToSavedPositionRequested;
        closedSettingsWindow.Closed -= OnSettingsWindowClosed;

        if (ReferenceEquals(settingsWindow, closedSettingsWindow))
        {
            settingsWindow = null;
        }

        if (IsVisible)
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Activate();
                    Focus();
                    WindowList.Focus();
                }),
                DispatcherPriority.ApplicationIdle);
        }
    }

    private void OnRemoteMoveRequested(OverlayAnchor anchor)
    {
        var workArea = GetCurrentWorkArea();
        temporaryOverlayWorkArea = workArea;
        temporaryOverlayPosition = OverlayPositionCalculator.Calculate(workArea, Width, Height, anchor);
        Left = temporaryOverlayPosition.Value.Left;
        Top = temporaryOverlayPosition.Value.Top;
    }

    private void OnReturnToSavedPositionRequested()
    {
        temporaryOverlayPosition = null;
        temporaryOverlayWorkArea = null;
        ApplyContentSizedBounds();
    }

    private void OnWindowSurfaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left ||
            e.OriginalSource is not DependencyObject source ||
            IsInteractiveSurface(source))
        {
            return;
        }

        try
        {
            DragMove();
            SaveDraggedOverlayPosition();
        }
        catch (InvalidOperationException)
        {
            // WPF can cancel DragMove when the pointer is released before it starts.
        }
    }

    private void SaveDraggedOverlayPosition()
    {
        var workArea = GetCurrentWorkArea();
        viewModel.SavedOverlayPosition = OverlayPositionCalculator.Capture(
            workArea,
            new OverlayPosition(Left, Top),
            Width,
            Height);
        temporaryOverlayPosition = null;
        temporaryOverlayWorkArea = null;
    }

    private static bool IsInteractiveSurface(DependencyObject source) =>
        FindAncestor<System.Windows.Controls.Primitives.ButtonBase>(source) is not null ||
        FindAncestor<System.Windows.Controls.Primitives.TextBoxBase>(source) is not null ||
        FindAncestor<Selector>(source) is not null ||
        FindAncestor<System.Windows.Controls.Primitives.ScrollBar>(source) is not null ||
        FindAncestor<Thumb>(source) is not null ||
        FindAncestor<ListBoxItem>(source) is not null;

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
