using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.App.ViewModels;

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

    public IReadOnlyList<WindowSnapshot> VisibleWindows =>
        WindowQuery.Apply(allWindows, SearchText, SelectedSortMode);

    public string WindowCountLabel => $"{VisibleWindows.Count} windows";

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
}
