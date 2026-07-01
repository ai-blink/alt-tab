using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class WindowQuery
{
    public static IReadOnlyList<WindowSnapshot> Apply(
        IEnumerable<WindowSnapshot> windows,
        string? searchText,
        WindowSortMode sortMode)
    {
        var query = windows;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            query = query.Where(window =>
                window.AppName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                window.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                window.MonitorName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = sortMode switch
        {
            WindowSortMode.App => query.OrderBy(window => window.AppName).ThenBy(window => window.Title),
            WindowSortMode.Monitor => query.OrderBy(window => window.MonitorName).ThenByDescending(window => window.LastActivatedAt),
            WindowSortMode.Title => query.OrderBy(window => window.Title),
            WindowSortMode.Favorites => query.OrderByDescending(window => window.IsFavorite).ThenByDescending(window => window.LastActivatedAt),
            _ => query.OrderByDescending(window => window.LastActivatedAt)
        };

        return query.ToList();
    }
}
