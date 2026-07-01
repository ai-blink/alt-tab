using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.App.ViewModels;

public enum OverlayThemeMode
{
    Transparent,
    Dark,
    Light
}

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IWindowCatalog windowCatalog;
    private IReadOnlyList<WindowSnapshot> allWindows;

    public MainWindowViewModel(IWindowCatalog windowCatalog)
    {
        this.windowCatalog = windowCatalog;
        allWindows = windowCatalog.GetOpenWindows();
        SelectedWindow = PickDefaultWindow();
    }

    public IReadOnlyList<SwitcherViewMode> ViewModes { get; } =
    [
        SwitcherViewMode.Grid,
        SwitcherViewMode.Compact,
        SwitcherViewMode.List
    ];

    public IReadOnlyList<WindowSortMode> SortModes { get; } =
    [
        WindowSortMode.Recent,
        WindowSortMode.App,
        WindowSortMode.Monitor,
        WindowSortMode.Title,
        WindowSortMode.Favorites
    ];

    public IReadOnlyList<OverlayThemeMode> AppearanceModes { get; } =
    [
        OverlayThemeMode.Transparent,
        OverlayThemeMode.Dark,
        OverlayThemeMode.Light
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibleWindows))]
    [NotifyPropertyChangedFor(nameof(WindowCountLabel))]
    private string? searchText;

    [ObservableProperty]
    private SwitcherViewMode selectedViewMode = SwitcherViewMode.Grid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibleWindows))]
    private WindowSortMode selectedSortMode = WindowSortMode.Recent;

    [ObservableProperty]
    private WindowSnapshot? selectedWindow;

    [ObservableProperty]
    private OverlayThemeMode selectedAppearanceMode = OverlayThemeMode.Transparent;

    public IReadOnlyList<WindowSnapshot> VisibleWindows =>
        WindowQuery.Apply(allWindows, SearchText, SelectedSortMode).Take(6).ToList();

    public string WindowCountLabel => $"{VisibleWindows.Count} windows";

    public Brush ShellBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(250, 248, 250, 252),
        OverlayThemeMode.Dark => Brush(248, 21, 24, 30),
        _ => Brush(210, 13, 16, 22)
    };

    public Brush HeaderBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(248, 255, 255, 255),
        OverlayThemeMode.Dark => Brush(232, 28, 31, 38),
        _ => Brush(144, 20, 24, 32)
    };

    public Brush CardBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(250, 255, 255, 255),
        OverlayThemeMode.Dark => Brush(238, 35, 38, 46),
        _ => Brush(164, 26, 30, 40)
    };

    public Brush SearchBackground => SelectedAppearanceMode switch
    {
        OverlayThemeMode.Light => Brush(255, 244, 246, 250),
        OverlayThemeMode.Dark => Brush(255, 16, 19, 25),
        _ => Brush(138, 8, 11, 17)
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
        OverlayThemeMode.Light => Brush(255, 236, 239, 245),
        OverlayThemeMode.Dark => Brush(255, 11, 14, 20),
        _ => Brush(126, 8, 11, 17)
    };

    public Brush PreviewLineBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(180, 255, 255, 255)
        : Brush(168, 255, 255, 255);

    public Brush PreviewShadeBrush => SelectedAppearanceMode == OverlayThemeMode.Light
        ? Brush(52, 12, 18, 28)
        : Brush(72, 255, 255, 255);

    public double ShellShadowOpacity => SelectedAppearanceMode == OverlayThemeMode.Transparent ? 0.45 : 0.28;

    [RelayCommand]
    private void Refresh()
    {
        allWindows = windowCatalog.GetOpenWindows();
        SelectedWindow = PickDefaultWindow();
        OnPropertyChanged(nameof(VisibleWindows));
        OnPropertyChanged(nameof(WindowCountLabel));
    }

    private WindowSnapshot? PickDefaultWindow() =>
        VisibleWindows.FirstOrDefault(window => window.IsActive) ?? VisibleWindows.FirstOrDefault();

    partial void OnSelectedAppearanceModeChanged(OverlayThemeMode value)
    {
        OnPropertyChanged(nameof(ShellBackground));
        OnPropertyChanged(nameof(HeaderBackground));
        OnPropertyChanged(nameof(CardBackground));
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

    private static SolidColorBrush Brush(byte alpha, byte red, byte green, byte blue)
    {
        var brush = new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        brush.Freeze();
        return brush;
    }
}
