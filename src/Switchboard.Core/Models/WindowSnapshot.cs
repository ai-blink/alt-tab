namespace Switchboard.Core.Models;

public sealed record WindowSnapshot(
    string Id,
    nint Handle,
    string AppName,
    string Title,
    string MonitorName,
    string ThumbnailLabel,
    string ThumbnailBrush,
    bool IsActive,
    bool IsFavorite,
    DateTimeOffset LastActivatedAt);
