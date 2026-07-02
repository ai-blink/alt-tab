using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class WindowSelection
{
    public static WindowSnapshot? EnsureVisible(
        IReadOnlyList<WindowSnapshot> windows,
        WindowSnapshot? selectedWindow)
    {
        if (windows.Count == 0)
        {
            return null;
        }

        if (selectedWindow is not null)
        {
            var matchingWindow = windows.FirstOrDefault(window => window.Id == selectedWindow.Id);

            if (matchingWindow is not null)
            {
                return matchingWindow;
            }
        }

        return windows[0];
    }

    public static WindowSnapshot? Move(
        IReadOnlyList<WindowSnapshot> windows,
        WindowSnapshot? selectedWindow,
        int offset)
    {
        if (windows.Count == 0)
        {
            return null;
        }

        var selectedIndex = FindSelectedIndex(windows, selectedWindow);

        if (selectedIndex < 0)
        {
            return offset < 0 ? windows[^1] : windows[0];
        }

        var nextIndex = (selectedIndex + offset) % windows.Count;

        if (nextIndex < 0)
        {
            nextIndex += windows.Count;
        }

        return windows[nextIndex];
    }

    private static int FindSelectedIndex(IReadOnlyList<WindowSnapshot> windows, WindowSnapshot? selectedWindow)
    {
        if (selectedWindow is null)
        {
            return -1;
        }

        for (var index = 0; index < windows.Count; index++)
        {
            if (windows[index].Id == selectedWindow.Id)
            {
                return index;
            }
        }

        return -1;
    }
}
