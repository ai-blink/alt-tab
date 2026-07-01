# Dev Roadmap

| Status | Milestone | Evidence |
|---|---|---|
| done | Initialize `.NET 10 + WPF` solution and project structure. | `dotnet build`, `dotnet test` pass |
| done | Preserve Stitch design references inside project. | `references/stitch/switchboard_premium_window_switcher` |
| in-progress | Build visibly polished mock-data switcher shell. | Build/test pass; visual acceptance pending |
| next | Fix launch visibility and run manual visual smoke. | User feedback: UI not yet apparent |
| next | Native top-level window enumeration. | `Switchboard.Native` provider |
| next | DWM thumbnail rendering path. | Native thumbnail host/prototype |
| later | Compact/List mode-specific templates. | App view template work |
| later | Settings persistence and favorite windows. | JSON or LiteDB decision |
