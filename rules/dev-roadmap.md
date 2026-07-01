# Dev Roadmap

| Status | Milestone | Evidence |
|---|---|---|
| done | Initialize `.NET 10 + WPF` solution and project structure. | `dotnet build`, `dotnet test` pass |
| done | Preserve Stitch design references inside project. | `references/stitch/switchboard_premium_window_switcher` |
| done | Build visibly polished mock-data switcher shell. | `dotnet build`, `dotnet test`, visual smoke PNG |
| done | Fix launch visibility and run manual visual smoke. | `notes/runs/2026-07-02_switchboard_visible_smoke.png` |
| done | Convert shell to compact transparent overlay with light/dark modes. | `notes/runs/2026-07-02_switchboard_compact_transparent_smoke.png` |
| next | Native top-level window enumeration. | `Switchboard.Native` provider |
| next | DWM thumbnail rendering path. | Native thumbnail host/prototype |
| later | Compact/List mode-specific templates. | App view template work |
| later | Settings persistence and favorite windows. | JSON or LiteDB decision |
