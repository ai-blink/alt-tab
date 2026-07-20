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

    private static MainWindowViewModel CreateViewModel(IWindowCatalog catalog, IWindowCloser? closer = null) =>
        new(catalog, new StubWindowActivator(), closer ?? new StubWindowCloser(), new StubSettingsStore());

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

    private sealed class StubSettingsStore : IUserSettingsStore
    {
        public UserSettings Load() => new();

        public void Save(UserSettings settings)
        {
        }
    }
}
