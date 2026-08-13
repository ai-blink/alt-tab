using Switchboard.App;
using Switchboard.App.ViewModels;
using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class MainWindowViewModelRefreshTests
{
    [Fact]
    public void RefreshWindows_does_not_notify_when_only_poll_timestamp_changed()
    {
        var initial = CreateWindow("Project", DateTimeOffset.UtcNow);
        var catalog = new StubWindowCatalog([initial]);
        var viewModel = CreateViewModel(catalog);
        var visibleWindowsNotifications = 0;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.VisibleWindows))
            {
                visibleWindowsNotifications++;
            }
        };
        catalog.Windows = [initial with { LastActivatedAt = initial.LastActivatedAt.AddSeconds(1) }];

        viewModel.RefreshWindows();

        Assert.Equal(0, visibleWindowsNotifications);
        Assert.Same(initial, viewModel.VisibleWindows[0]);
    }

    [Fact]
    public void RefreshWindows_notifies_when_window_presentation_changes()
    {
        var initial = CreateWindow("Project", DateTimeOffset.UtcNow);
        var catalog = new StubWindowCatalog([initial]);
        var viewModel = CreateViewModel(catalog);
        var visibleWindowsNotifications = 0;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.VisibleWindows))
            {
                visibleWindowsNotifications++;
            }
        };
        catalog.Windows = [initial with { Title = "Renamed project" }];

        viewModel.RefreshWindows();

        Assert.Equal(1, visibleWindowsNotifications);
        Assert.Equal("Renamed project", viewModel.VisibleWindows[0].Title);
    }

    [Fact]
    public void CloseWindowCommand_requests_close_for_supplied_window()
    {
        var window = CreateWindow("Project", DateTimeOffset.UtcNow);
        var closer = new StubWindowCloser();
        var viewModel = CreateViewModel(new StubWindowCatalog([window]), closer);

        viewModel.CloseWindowCommand.Execute(window);

        Assert.Same(window, closer.ClosedWindow);
    }

    [Fact]
    public void Compact_overlay_preferences_are_loaded_and_saved()
    {
        var settingsStore = new StubSettingsStore(new UserSettings
        {
            IsCompactOverlayEnabled = true,
            CompactOverlayPlacement = OverlayPlacement.TopLeft
        });
        var viewModel = CreateViewModel(
            new StubWindowCatalog([CreateWindow("Project", DateTimeOffset.UtcNow)]),
            settingsStore: settingsStore);

        Assert.True(viewModel.IsCompactOverlayEnabled);
        Assert.Equal(OverlayPlacement.TopLeft, viewModel.SelectedCompactOverlayPlacement);

        viewModel.IsCompactOverlayEnabled = false;
        viewModel.SelectedCompactOverlayPlacement = OverlayPlacement.BottomRight;

        Assert.False(settingsStore.CurrentSettings.IsCompactOverlayEnabled);
        Assert.Equal(OverlayPlacement.BottomRight, settingsStore.CurrentSettings.CompactOverlayPlacement);
    }

    [Theory]
    [InlineData(OverlayScalePreset.Sixty, 0.6)]
    [InlineData(OverlayScalePreset.Seventy, 0.7)]
    [InlineData(OverlayScalePreset.Eighty, 0.8)]
    [InlineData(OverlayScalePreset.Hundred, 1.0)]
    [InlineData(OverlayScalePreset.OneTwentyFive, 1.25)]
    [InlineData(OverlayScalePreset.OneFifty, 1.5)]
    [InlineData(OverlayScalePreset.TwoHundred, 2.0)]
    public void Overlay_scale_presets_apply_to_the_entire_presentation(
        OverlayScalePreset preset,
        double expectedScale)
    {
        var viewModel = CreateViewModel(new StubWindowCatalog([CreateWindow("Project", DateTimeOffset.UtcNow)]));

        viewModel.SelectedOverlayScalePreset = preset;
        viewModel.IsCompactOverlayEnabled = false;
        var regularScale = viewModel.PresentationScale;
        viewModel.IsCompactOverlayEnabled = true;

        Assert.Equal(expectedScale, regularScale);
        Assert.Equal(expectedScale, viewModel.PresentationScale);
    }

    [Fact]
    public void Overlay_scale_options_include_compact_sixty_and_seventy_presets()
    {
        var viewModel = CreateViewModel(new StubWindowCatalog([CreateWindow("Project", DateTimeOffset.UtcNow)]));

        Assert.Equal(
            [
                OverlayScalePreset.Sixty,
                OverlayScalePreset.Seventy,
                OverlayScalePreset.Eighty,
                OverlayScalePreset.Hundred,
                OverlayScalePreset.OneTwentyFive,
                OverlayScalePreset.OneFifty,
                OverlayScalePreset.TwoHundred
            ],
            viewModel.OverlayScalePresets);
    }

    [Theory]
    [InlineData(OverlayScalePreset.Fifty, OverlayScalePreset.Sixty)]
    [InlineData(OverlayScalePreset.Ninety, OverlayScalePreset.Hundred)]
    [InlineData(OverlayScalePreset.OneTwenty, OverlayScalePreset.OneTwentyFive)]
    public void Legacy_overlay_scale_is_normalized_when_settings_are_loaded(
        OverlayScalePreset storedPreset,
        OverlayScalePreset expectedPreset)
    {
        var settingsStore = new StubSettingsStore(new UserSettings
        {
            SelectedOverlayScalePreset = storedPreset
        });

        var viewModel = CreateViewModel(
            new StubWindowCatalog([CreateWindow("Project", DateTimeOffset.UtcNow)]),
            settingsStore: settingsStore);

        Assert.Equal(expectedPreset, viewModel.SelectedOverlayScalePreset);
    }

    [Fact]
    public void Saved_overlay_position_is_loaded_and_saved()
    {
        var savedPosition = new OverlayPositionPreference
        {
            MonitorDeviceName = "DISPLAY2",
            Anchor = OverlayAnchor.BottomRight,
            OffsetX = -12,
            OffsetY = 8
        };
        var settingsStore = new StubSettingsStore(new UserSettings
        {
            SavedOverlayPosition = savedPosition
        });
        var viewModel = CreateViewModel(
            new StubWindowCatalog([CreateWindow("Project", DateTimeOffset.UtcNow)]),
            settingsStore: settingsStore);

        Assert.Equal(savedPosition, viewModel.SavedOverlayPosition);

        var updatedPosition = savedPosition with { Anchor = OverlayAnchor.TopLeft, OffsetX = 4 };
        viewModel.SavedOverlayPosition = updatedPosition;

        Assert.Equal(updatedPosition, settingsStore.CurrentSettings.SavedOverlayPosition);
    }

    private static MainWindowViewModel CreateViewModel(
        IWindowCatalog catalog,
        IWindowCloser? closer = null,
        IUserSettingsStore? settingsStore = null) =>
        new(catalog, new StubWindowActivator(), closer ?? new StubWindowCloser(), settingsStore ?? new StubSettingsStore());

    private static WindowSnapshot CreateWindow(string title, DateTimeOffset lastActivatedAt) =>
        new("editor", 1, "Editor", title, "Primary", "CODE", "#111111", true, false, lastActivatedAt);

    private sealed class StubWindowCatalog(IReadOnlyList<WindowSnapshot> windows) : IWindowCatalog
    {
        public IReadOnlyList<WindowSnapshot> Windows { get; set; } = windows;

        public IReadOnlyList<WindowSnapshot> GetOpenWindows() => Windows;
    }

    private sealed class StubWindowActivator : IWindowActivator
    {
        public bool TryActivate(WindowSnapshot window) => true;
    }

    private sealed class StubWindowCloser : IWindowCloser
    {
        public WindowSnapshot? ClosedWindow { get; private set; }

        public bool TryClose(WindowSnapshot window)
        {
            ClosedWindow = window;
            return true;
        }
    }

    private sealed class StubSettingsStore(UserSettings? settings = null) : IUserSettingsStore
    {
        public UserSettings CurrentSettings { get; private set; } = settings ?? new();

        public UserSettings Load() => CurrentSettings;

        public void Save(UserSettings settings) => CurrentSettings = settings;
    }
}
