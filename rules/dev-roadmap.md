# Dev Roadmap

| Status | Milestone | Evidence |
|---|---|---|
| done | Initialize `.NET 10 + WPF` solution and project structure. | `dotnet build`, `dotnet test` pass |
| done | Preserve Stitch design references inside project. | `references/stitch/switchboard_premium_window_switcher` |
| done | Build visibly polished mock-data switcher shell. | `dotnet build`, `dotnet test`, visual smoke PNG |
| done | Fix launch visibility and run manual visual smoke. | `notes/runs/2026-07-02_switchboard_visible_smoke.png` |
| done | Convert shell to compact transparent overlay with light/dark modes. | `notes/runs/2026-07-02_switchboard_compact_transparent_smoke.png` |
| done | Native top-level window enumeration. | `Switchboard.Native` provider |
| done | DWM thumbnail rendering path. | `notes/runs/2026-07-02_switchboard_dwm_thumbnail_smoke.png` |
| done | Rebalance cards around thumbnails as the primary content. | `notes/runs/2026-07-02_switchboard_thumbnail_first_smoke.png` |
| done | Double thumbnail height while preserving the 3x2 switcher layout. | `notes/runs/2026-07-02_switchboard_thumbnail_double_height_smoke.png` |
| done | Keyboard selection and foreground activation. | `dotnet test`, `notes/runs/2026-07-02_switchboard_keyboard_activation_smoke.png` |
| done | Compact/List mode-specific templates. | `notes/runs/2026-07-02_switchboard_view_modes_list_smoke.png` |
| done | Show all windows with in-overlay scrolling, soft DWM thumbnail fit, and double-click activation. | `notes/runs/2026-07-02_switchboard_all_windows_list_smoke.png` |
| done | Size the overlay and grid columns to the current window count instead of relying on visible scrolling. | `notes/runs/2026-07-02_switchboard_count_sized_desktop_smoke.png` |
| next | Add compact in-overlay settings for thumbnail scale, sizing policy, and default view mode candidates. | Build/test plus settings-panel visual smoke |
| later | Foreground activation fallback UX. | Handle Windows foreground-lock failures gracefully |
| later | Settings persistence and favorite windows. | JSON or LiteDB decision |
