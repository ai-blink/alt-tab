using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media;
using Switchboard.App;
using Switchboard.Core.Models;
using Switchboard.Core.Services;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace Switchboard.App.ViewModels;

public enum OverlayThemeMode
{
    Transparent,
    Dark,
    Light
}

public sealed record HotkeyKeyOption(SwitcherHotkeyKey Value, string Label);

public sealed record OverlayPlacementOption(OverlayPlacement Value, string Label);

public partial class MainWindowViewModel : ObservableObject
{
    private const double BaseGridCardWidth = 274;
    private const double BaseGridPreviewHeight = 160;
    private const double GridCaptionHeight = 28;
    private const double BaseCompactCardWidth = 224;
    private const double BaseCompactCardHeight = 96;
    private const double BaseListWidth = 600;
    private const double BaseListPreviewWidth = 82;
    private const double BaseListRowHeight = 54;
    private const double ItemHorizontalGap = 6;
    private const double ItemVerticalGap = 8;
    private const double SelectionFramePadding = 8;

    private readonly IWindowCatalog windowCatalog;
    private readonly IWindowActivator windowActivator;
    private readonly IWindowCloser windowCloser;
    private readonly IUserSettingsStore userSettingsStore;
    private IReadOnlyList<WindowSnapshot> allWindows;
    private bool isUpdatingHotkeyModifiers;

    public MainWindowViewModel(
        IWindowCatalog windowCatalog,
        IWindowActivator windowActivator,
        IWindowCloser windowCloser,
        IUserSettingsStore userSettingsStore)
    {
        this.windowCatalog = windowCatalog;
        this.windowActivator = windowActivator;
        this.windowCloser = windowCloser;
        this.userSettingsStore = userSettingsStore;
        allWindows = windowCatalog.GetOpenWindows();
        ApplyUserSettings(userSettingsStore.Load());
        SelectedWindow = PickDefaultWindow();
        PropertyChanged += OnViewModelPropertyChanged;
    }

    public IReadOnlyList<SwitcherViewMode> ViewModes { get; } =
    [
        SwitcherViewMode.Grid,
        SwitcherViewMode.Compact,
        SwitcherViewMode.List
    ];

    public IReadOnlyList<OverlayThemeMode> AppearanceModes { get; } =
    [
        OverlayThemeMode.Transparent,
        OverlayThemeMode.Dark,
        OverlayThemeMode.Light
    ];

    public IReadOnlyList<ThumbnailScalePreset> ThumbnailScalePresets { get; } =
    [
        ThumbnailScalePreset.Normal,
        ThumbnailScalePreset.Large,
        ThumbnailScalePreset.ExtraLarge
    ];

    public IReadOnlyList<OverlayOpacityPreset> OverlayOpacityPresets { get; } =
    [
        OverlayOpacityPreset.TwentyFive,
        OverlayOpacityPreset.Fifty,
        OverlayOpacityPreset.SeventyFive,
        OverlayOpacityPreset.Ninety
    ];

    public IReadOnlyList<OverlayScalePreset> OverlayScalePresets { get; } =
    [
        OverlayScalePreset.Fifty,
        OverlayScalePreset.Seventy,
        OverlayScalePreset.Ninety,
        OverlayScalePreset.Hundred,
        OverlayScalePreset.OneTwenty
    ];

    public IReadOnlyList<OverlayPlacementOption> CompactOverlayPlacementOptions { get; } =
    [
        new(OverlayPlacement.BottomLeft, "좌측 아래"),
        new(OverlayPlacement.BottomRight, "우측 아래"),
        new(OverlayPlacement.TopLeft, "좌측 위"),
        new(OverlayPlacement.TopRight, "우측 위"),
        new(OverlayPlacement.Center, "가운데")
    ];

    public IReadOnlyList<SwitcherSizingPolicy> SizingPolicies { get; } =
    [
        SwitcherSizingPolicy.Auto,
        SwitcherSizingPolicy.Dense
    ];

    public IReadOnlyList<SwitcherHotkeyModifier> HotkeyModifiers { get; } =
    [
        SwitcherHotkeyModifier.Ctrl,
        SwitcherHotkeyModifier.Alt,
        SwitcherHotkeyModifier.Shift,
        SwitcherHotkeyModifier.Win
    ];

    public IReadOnlyList<HotkeyKeyOption> HotkeyKeyOptions { get; } = CreateHotkeyKeyOptions();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibleWindows))]
    [NotifyPropertyChangedFor(nameof(WindowCountLabel))]
    private string? searchText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ItemSlotWidth))]
    [NotifyPropertyChangedFor(nameof(ItemSlotHeight))]
    private SwitcherViewMode selectedViewMode = SwitcherViewMode.Grid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibleWindows))]
    private WindowSortMode selectedSortMode = WindowSortMode.Recent;

    [ObservableProperty]
    private WindowSnapshot? selectedWindow;

    [ObservableProperty]
    private OverlayThemeMode selectedAppearanceMode = OverlayThemeMode.Transparent;

    [ObservableProperty]
    private OverlayOpacityPreset selectedOverlayOpacityPreset = OverlayOpacityPreset.Ninety;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AppScale))]
    [NotifyPropertyChangedFor(nameof(PresentationScale))]
    private OverlayScalePreset selectedOverlayScalePreset = OverlayScalePreset.Ninety;

    [ObservableProperty]
    private int gridColumnCount = 3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ThumbnailScale))]
    [NotifyPropertyChangedFor(nameof(ThumbnailScaleLabel))]
    [NotifyPropertyChangedFor(nameof(GridCardWidth))]
    [NotifyPropertyChangedFor(nameof(GridCardHeight))]
    [NotifyPropertyChangedFor(nameof(CompactCardWidth))]
    [NotifyPropertyChangedFor(nameof(CompactCardHeight))]
    [NotifyPropertyChangedFor(nameof(ListWidth))]
    [NotifyPropertyChangedFor(nameof(ListRowHeight))]
    [NotifyPropertyChangedFor(nameof(ListPreviewColumnWidth))]
    [NotifyPropertyChangedFor(nameof(ItemSlotWidth))]
    [NotifyPropertyChangedFor(nameof(ItemSlotHeight))]
    private ThumbnailScalePreset selectedThumbnailScalePreset = ThumbnailScalePreset.Normal;

    [ObservableProperty]
    private SwitcherSizingPolicy selectedSizingPolicy = SwitcherSizingPolicy.Auto;

    [ObservableProperty]
    private SwitcherViewMode defaultViewMode = SwitcherViewMode.Grid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedHotkeyLabel))]
    private SwitcherHotkeyModifier selectedFirstHotkeyModifier = SwitcherHotkeyModifier.Ctrl;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedHotkeyLabel))]
    private SwitcherHotkeyModifier selectedSecondHotkeyModifier = SwitcherHotkeyModifier.Alt;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedHotkeyLabel))]
    private SwitcherHotkeyKey selectedHotkeyKey = SwitcherHotkeyKey.Space;

    [ObservableProperty]
    private bool isSettingsOpen;

    [ObservableProperty]
    private bool isAlwaysOnTop = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PresentationScale))]
    private bool isCompactOverlayEnabled;

    [ObservableProperty]
    private OverlayPlacement selectedCompactOverlayPlacement = OverlayPlacement.BottomLeft;

    public IReadOnlyList<WindowSnapshot> VisibleWindows =>
        WindowQuery.Apply(allWindows, SearchText, SelectedSortMode).ToList();

    public string WindowCountLabel => $"{VisibleWindows.Count} windows";

    public double ThumbnailScale => SelectedThumbnailScalePreset switch
    {
        ThumbnailScalePreset.Large => 1.1,
        ThumbnailScalePreset.ExtraLarge => 1.2,
        _ => 1.0
    };

    public string ThumbnailScaleLabel => $"{ThumbnailScale:0.0}x";

    public double AppScale => SelectedOverlayScalePreset switch
    {
        OverlayScalePreset.Fifty => 0.5,
        OverlayScalePreset.Seventy => 0.7,
        OverlayScalePreset.Hundred => 1.0,
        OverlayScalePreset.OneTwenty => 1.2,
        _ => 0.9
    };

    public double PresentationScale => AppScale * (IsCompactOverlayEnabled ? 0.7 : 1.0);

    public double GridCardWidth => Math.Round(BaseGridCardWidth * ThumbnailScale);

    public double GridCardHeight => GridCaptionHeight + Math.Round(BaseGridPreviewHeight * ThumbnailScale);

    public double CompactCardWidth => Math.Round(BaseCompactCardWidth * ThumbnailScale);

    public double CompactCardHeight => Math.Round(BaseCompactCardHeight * ThumbnailScale);

    public double ListWidth => Math.Round(BaseListWidth * ThumbnailScale);

    public double ListRowHeight => Math.Round(BaseListRowHeight * ThumbnailScale);

    public GridLength ListPreviewColumnWidth => new(Math.Round(BaseListPreviewWidth * ThumbnailScale));

    public double ItemSlotWidth => SelectedViewMode switch
    {
        SwitcherViewMode.Compact => CompactCardWidth + ItemHorizontalGap + SelectionFramePadding,
        SwitcherViewMode.List => ListWidth + ItemHorizontalGap + SelectionFramePadding,
        _ => GridCardWidth + ItemHorizontalGap + SelectionFramePadding
    };

    public double ItemSlotHeight => SelectedViewMode switch
    {
        SwitcherViewMode.Compact => CompactCardHeight + ItemVerticalGap + SelectionFramePadding,
        SwitcherViewMode.List => ListRowHeight + ItemVerticalGap + SelectionFramePadding,
        _ => GridCardHeight + ItemVerticalGap + SelectionFramePadding
    };

    public Brush ShellBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(250), 248, 250, 252),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(248), 21, 24, 30),
        _ => Brush(ScaledAlpha(210), 13, 16, 22)
    };

    public Brush HeaderBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(248), 255, 255, 255),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(232), 28, 31, 38),
        _ => Brush(ScaledAlpha(144), 20, 24, 32)
    };

    public Brush CardBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(250), 255, 255, 255),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(238), 35, 38, 46),
        _ => Brush(ScaledAlpha(164), 26, 30, 40)
    };

    public Brush PopoverBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(252, minimum: 230), 255, 255, 255),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(248, minimum: 220), 28, 31, 38),
        _ => Brush(ScaledAlpha(236, minimum: 210), 20, 24, 32)
    };

    public Brush SearchBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(255), 244, 246, 250),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(255), 16, 19, 25),
        _ => Brush(ScaledAlpha(138), 8, 11, 17)
    };

    public Brush BorderBrush => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(255, 196, 202, 214),
        OverlayThemeMode.Dark => Brush(255, 69, 76, 91),
        _ => Brush(150, 174, 188, 210)
    };

    public Brush TextBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(255, 25, 28, 34)
        : Brush(255, 246, 248, 252);

    public Brush MutedTextBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(255, 78, 86, 102)
        : Brush(255, 183, 192, 207);

    public Brush AccentBrush => Brush(255, 84, 163, 255);

    public Brush AccentSoftBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(255, 218, 236, 255)
        : Brush(112, 47, 122, 218);

    public Brush SegmentBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(ScaledAlpha(255), 236, 239, 245),
        OverlayThemeMode.Dark => Brush(ScaledAlpha(255), 11, 14, 20),
        _ => Brush(ScaledAlpha(126), 8, 11, 17)
    };

    public Brush PreviewLineBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(180, 255, 255, 255)
        : Brush(168, 255, 255, 255);

    public Brush PreviewShadeBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(52, 12, 18, 28)
        : Brush(72, 255, 255, 255);

    public double ShellShadowOpacity =>
        (SelectedAppearanceMode == OverlayThemeMode.Transparent ? 0.45 : 0.28) * OpacityScale;

    public bool AreDwmThumbnailsVisible => true;

    public string SelectedHotkeyLabel =>
        $"{FormatHotkeyPart(SelectedFirstHotkeyModifier)}+{FormatHotkeyPart(SelectedSecondHotkeyModifier)}+{FormatHotkeyPart(SelectedHotkeyKey)}";

    public void SelectNextWindow() => MoveSelection(1);

    public void SelectPreviousWindow() => MoveSelection(-1);

    public void SelectWindowAbove() => MoveSelection(-VerticalSelectionStep);

    public void SelectWindowBelow() => MoveSelection(VerticalSelectionStep);

    public bool TryActivateSelectedWindow() =>
        SelectedWindow is not null && windowActivator.TryActivate(SelectedWindow);

    public void SetGridColumnCount(int columnCount) =>
        GridColumnCount = Math.Max(1, columnCount);

    public void RefreshWindows()
    {
        var refreshedWindows = windowCatalog.GetOpenWindows();

        if (HasSamePresentation(allWindows, refreshedWindows))
        {
            return;
        }

        allWindows = refreshedWindows;
        EnsureSelectedWindowIsVisible();
        OnPropertyChanged(nameof(VisibleWindows));
        OnPropertyChanged(nameof(WindowCountLabel));
    }

    private static bool HasSamePresentation(
        IReadOnlyList<WindowSnapshot> current,
        IReadOnlyList<WindowSnapshot> refreshed)
    {
        if (current.Count != refreshed.Count)
        {
            return false;
        }

        for (var index = 0; index < current.Count; index++)
        {
            var left = current[index];
            var right = refreshed[index];

            if (left.Handle != right.Handle ||
                left.AppName != right.AppName ||
                left.Title != right.Title ||
                left.MonitorName != right.MonitorName ||
                left.ThumbnailLabel != right.ThumbnailLabel ||
                left.ThumbnailBrush != right.ThumbnailBrush ||
                left.IsActive != right.IsActive ||
                left.IsFavorite != right.IsFavorite)
            {
                return false;
            }
        }

        return true;
    }

    [RelayCommand]
    private void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;

    [RelayCommand]
    private void CloseSettings() => IsSettingsOpen = false;

    [RelayCommand]
    private void Refresh()
    {
        RefreshWindows();
    }

    [RelayCommand]
    private void CloseWindow(WindowSnapshot? window)
    {
        if (window is not null)
        {
            _ = windowCloser.TryClose(window);
        }
    }

    private WindowSnapshot? PickDefaultWindow() =>
        VisibleWindows.FirstOrDefault(window => window.IsActive) ?? VisibleWindows.FirstOrDefault();

    private int VerticalSelectionStep => GridColumnCount;

    private void MoveSelection(int offset) =>
        SelectedWindow = WindowSelection.Move(VisibleWindows, SelectedWindow, offset);

    private void EnsureSelectedWindowIsVisible() =>
        SelectedWindow = WindowSelection.EnsureVisible(VisibleWindows, SelectedWindow);

    partial void OnSearchTextChanged(string? value) => EnsureSelectedWindowIsVisible();

    partial void OnSelectedSortModeChanged(WindowSortMode value) => EnsureSelectedWindowIsVisible();

    partial void OnSelectedAppearanceModeChanged(OverlayThemeMode value)
    {
        RefreshOverlayBrushes();
    }

    partial void OnSelectedOverlayOpacityPresetChanged(OverlayOpacityPreset value)
    {
        RefreshOverlayBrushes();
    }

    partial void OnSelectedFirstHotkeyModifierChanged(SwitcherHotkeyModifier value) =>
        EnsureDistinctHotkeyModifiers(changedFirstModifier: true);

    partial void OnSelectedSecondHotkeyModifierChanged(SwitcherHotkeyModifier value) =>
        EnsureDistinctHotkeyModifiers(changedFirstModifier: false);

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (IsPersistedSetting(e.PropertyName))
        {
            userSettingsStore.Save(CreateUserSettings());
        }
    }

    private static bool IsPersistedSetting(string? propertyName) =>
        propertyName is
            nameof(SelectedViewMode) or
            nameof(SelectedSortMode) or
            nameof(SelectedAppearanceMode) or
            nameof(SelectedOverlayOpacityPreset) or
            nameof(SelectedOverlayScalePreset) or
            nameof(SelectedThumbnailScalePreset) or
            nameof(SelectedSizingPolicy) or
            nameof(DefaultViewMode) or
            nameof(SelectedFirstHotkeyModifier) or
            nameof(SelectedSecondHotkeyModifier) or
            nameof(SelectedHotkeyKey) or
            nameof(IsCompactOverlayEnabled) or
            nameof(SelectedCompactOverlayPlacement) or
            nameof(IsAlwaysOnTop);

    private void ApplyUserSettings(UserSettings settings)
    {
        SelectedViewMode = settings.SelectedViewMode;
        SelectedSortMode = settings.SelectedSortMode;
        SelectedAppearanceMode = settings.SelectedAppearanceMode;
        SelectedOverlayOpacityPreset = settings.SelectedOverlayOpacityPreset;
        SelectedOverlayScalePreset = settings.SelectedOverlayScalePreset;
        SelectedThumbnailScalePreset = settings.SelectedThumbnailScalePreset;
        SelectedSizingPolicy = settings.SelectedSizingPolicy;
        DefaultViewMode = settings.DefaultViewMode;
        SelectedFirstHotkeyModifier = settings.SelectedFirstHotkeyModifier;
        SelectedSecondHotkeyModifier = settings.SelectedSecondHotkeyModifier;
        SelectedHotkeyKey = settings.SelectedHotkeyKey;
        IsCompactOverlayEnabled = settings.IsCompactOverlayEnabled;
        SelectedCompactOverlayPlacement = settings.CompactOverlayPlacement;
        IsAlwaysOnTop = settings.IsAlwaysOnTop;
    }

    private UserSettings CreateUserSettings() => new()
    {
        SelectedViewMode = SelectedViewMode,
        SelectedSortMode = SelectedSortMode,
        SelectedAppearanceMode = SelectedAppearanceMode,
        SelectedOverlayOpacityPreset = SelectedOverlayOpacityPreset,
        SelectedOverlayScalePreset = SelectedOverlayScalePreset,
        SelectedThumbnailScalePreset = SelectedThumbnailScalePreset,
        SelectedSizingPolicy = SelectedSizingPolicy,
        DefaultViewMode = DefaultViewMode,
        SelectedFirstHotkeyModifier = SelectedFirstHotkeyModifier,
        SelectedSecondHotkeyModifier = SelectedSecondHotkeyModifier,
        SelectedHotkeyKey = SelectedHotkeyKey,
        IsCompactOverlayEnabled = IsCompactOverlayEnabled,
        CompactOverlayPlacement = SelectedCompactOverlayPlacement,
        IsAlwaysOnTop = IsAlwaysOnTop
    };

    private void EnsureDistinctHotkeyModifiers(bool changedFirstModifier)
    {
        if (isUpdatingHotkeyModifiers || SelectedFirstHotkeyModifier != SelectedSecondHotkeyModifier)
        {
            return;
        }

        isUpdatingHotkeyModifiers = true;

        try
        {
            if (changedFirstModifier)
            {
                SelectedSecondHotkeyModifier = GetFallbackModifier(SelectedFirstHotkeyModifier);
            }
            else
            {
                SelectedFirstHotkeyModifier = GetFallbackModifier(SelectedSecondHotkeyModifier);
            }
        }
        finally
        {
            isUpdatingHotkeyModifiers = false;
        }
    }

    private static SwitcherHotkeyModifier GetFallbackModifier(SwitcherHotkeyModifier selected) =>
        selected == SwitcherHotkeyModifier.Ctrl
            ? SwitcherHotkeyModifier.Alt
            : SwitcherHotkeyModifier.Ctrl;

    private static string FormatHotkeyPart(SwitcherHotkeyModifier modifier) => modifier switch
    {
        SwitcherHotkeyModifier.Ctrl => "Ctrl",
        SwitcherHotkeyModifier.Alt => "Alt",
        SwitcherHotkeyModifier.Shift => "Shift",
        _ => "Win"
    };

    private static IReadOnlyList<HotkeyKeyOption> CreateHotkeyKeyOptions()
    {
        var keys = Enum.GetValues<SwitcherHotkeyKey>();
        return keys.Select(key => new HotkeyKeyOption(key, FormatHotkeyPart(key))).ToList();
    }

    private static string FormatHotkeyPart(SwitcherHotkeyKey key) => key switch
    {
        SwitcherHotkeyKey.Space => "Space",
        SwitcherHotkeyKey.Tab => "Tab",
        SwitcherHotkeyKey.Enter => "Enter",
        >= SwitcherHotkeyKey.D0 and <= SwitcherHotkeyKey.D9 => ((int)key - (int)SwitcherHotkeyKey.D0).ToString(),
        _ => key.ToString()
    };

    private void RefreshOverlayBrushes()
    {
        OnPropertyChanged(nameof(ShellBackground));
        OnPropertyChanged(nameof(HeaderBackground));
        OnPropertyChanged(nameof(CardBackground));
        OnPropertyChanged(nameof(PopoverBackground));
        OnPropertyChanged(nameof(SearchBackground));
        OnPropertyChanged(nameof(BorderBrush));
        OnPropertyChanged(nameof(TextBrush));
        OnPropertyChanged(nameof(MutedTextBrush));
        OnPropertyChanged(nameof(AccentBrush));
        OnPropertyChanged(nameof(AccentSoftBrush));
        OnPropertyChanged(nameof(SegmentBackground));
        OnPropertyChanged(nameof(PreviewLineBrush));
        OnPropertyChanged(nameof(PreviewShadeBrush));
        OnPropertyChanged(nameof(ShellShadowOpacity));
    }

    private double OpacityScale => SelectedOverlayOpacityPreset switch
    {
        OverlayOpacityPreset.TwentyFive => 0.25,
        OverlayOpacityPreset.Fifty => 0.5,
        OverlayOpacityPreset.SeventyFive => 0.75,
        _ => 0.9
    };

    private byte ScaledAlpha(byte alpha) =>
        (byte)Math.Clamp((int)Math.Round(alpha * OpacityScale), 0, byte.MaxValue);

    private byte ScaledAlpha(byte alpha, byte minimum) =>
        (byte)Math.Clamp(Math.Max(minimum, (int)Math.Round(alpha * OpacityScale)), 0, byte.MaxValue);

    private static SolidColorBrush Brush(byte alpha, byte red, byte green, byte blue)
    {
        var brush = new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        brush.Freeze();
        return brush;
    }
}
