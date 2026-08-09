# Dev Roadmap

| Status | Milestone | Evidence |
|---|---|---|
| done | Initialize `.NET 10 + WPF` App/Core/Native/Tests structure and preserve Stitch references. | `dotnet build`, `dotnet test`, `references/stitch/` |
| done | Deliver the compact transparent WPF overlay with Grid/Compact/List views and settings. | `notes/runs/2026-07-02_*`, `notes/runs/2026-07-03_*` |
| done | Enumerate all candidate Win32 windows and render full-source DWM thumbnails. | Native provider, `DwmThumbnailPreview`, visual smoke artifacts |
| done | Add keyboard/mouse activation, sorting, tray residency, configurable hotkey registration, and settings persistence. | App runtime smokes and `%AppData%` persistence probe |
| done | Size rows, columns, and overlay bounds responsively without a 25-window accessibility ceiling. | `SwitcherLayoutCalculatorTests`, 30-window query regression |
| done | Stabilize visible-state catalog polling without rebuilding unchanged WPF/DWM visuals. | `MainWindowViewModelRefreshTests` |
| done | Make Alt+Tab toggle the overlay once per gesture and restore the previous foreground window. | `AltTabKeyFilterTests`, 10-gesture runtime smoke |
| done | Separate transient foreground presentation from persistent always-on-top policy. | 0 foreground/topmost failures; `WS_EX_TOPMOST=0` when disabled |
| done | Add always-visible per-card window close controls using standard `WM_CLOSE`. | `IWindowCloser`, `CloseWindowCommand`, 13 passing tests |
| done | Unify overlay scaling at 80/100/125/150/200%, keep compact position and automatic layout, and prevent DWM previews from escaping the list viewport. | `MainWindowViewModelRefreshTests`, `SwitcherLayoutCalculatorTests`, 41 passing Release tests |
| later | Add user-visible configurable-hotkey collision feedback. | Report reserved/already-registered Win32 combinations |
| later | Replace visible-state polling with Win32 event-driven catalog updates. | Preserve stable visual identity while reducing background work |
| later | Persist favorite windows. | Choose JSON or LiteDB storage |
| later | Add elevated/security-desktop activation fallback UX. | Explain foreground limitations without silent failure |
