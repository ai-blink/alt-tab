# Dev Roadmap

| Status | Milestone | Evidence |
|---|---|---|
| released — UI check pending | `v0.4.0` Portable: multilingual UI, Alt+Tab reliability (elevated sign-in task, hook self-healing), app icons and thumbnail icons, simplified toolbar. | GitHub Release `v0.4.0`; 110 Release tests; asset SHA-256 matches |
| done — sign-in check pending | Keep Alt+Tab on Switchboard over elevated windows and during brief stalls (elevated sign-in task, single instance, hook responsiveness, diagnostics log, lost key-up recovery). | 2026-09-27 94 tests; elevated vs standard probe; task-verify register/delete |
| done — UI check pending | Overlay UX from mocks: app icons + 5/6/7-char app names (duplicates hidden), simplified toolbar (2A), stronger selection with hover/selected 24px close (3A). | 2026-09-27 110 tests; Grid capture; `notes/mocks/2026-09-27_overlay-ux/` |
| done — UI check pending | Localize the app UI into Korean, English, Simplified Chinese, and Japanese with a live Language setting. | 2026-09-27 build 0 warnings/errors, 87 tests incl. four-way `LocalizationTests` key parity; ja/zh-Hans startup smoke |
| done — UI check pending | Add an 80/100/125/150% window title label scale preset across Grid/Compact/List. | `048d7fc`; `MainWindowViewModelRefreshTests`, `UserSettingsJsonTests` |
| released — UI check pending | Replace the settings popup with a sidebar modal and persist overlay drag position with one-shot 9-direction movement plus saved-position return. | Public `v0.3.1` Portable release; 2026-08-18 Release build: 0 warnings/errors, 57 tests pass; settings-modal clipping and sidebar status-card expansion fixed. |
| done | Initialize `.NET 10 + WPF` App/Core/Native/Tests structure and preserve Stitch references. | `dotnet build`, `dotnet test`, `references/stitch/` |
| done | Deliver the compact transparent WPF overlay with Grid/Compact/List views and settings. | `notes/runs/2026-07-02_*`, `notes/runs/2026-07-03_*` |
| done | Enumerate all candidate Win32 windows and render full-source DWM thumbnails. | Native provider, `DwmThumbnailPreview`, visual smoke artifacts |
| done | Add keyboard/mouse activation, sorting, tray residency, configurable hotkey registration, and settings persistence. | App runtime smokes and `%AppData%` persistence probe |
| done | Size rows, columns, and overlay bounds responsively without a 25-window accessibility ceiling. | `SwitcherLayoutCalculatorTests`, 30-window query regression |
| done | Stabilize visible-state catalog polling without rebuilding unchanged WPF/DWM visuals. | `MainWindowViewModelRefreshTests` |
| done | Make Alt+Tab toggle the overlay once per gesture and restore the previous foreground window. | `AltTabKeyFilterTests`, 10-gesture runtime smoke |
| done | Separate transient foreground presentation from persistent always-on-top policy. | 0 foreground/topmost failures; `WS_EX_TOPMOST=0` when disabled |
| done | Add always-visible per-card window close controls using standard `WM_CLOSE`. | `IWindowCloser`, `CloseWindowCommand`, 13 passing tests |
| done | Unify overlay scaling at 60/70/80/100/125/150/200%, keep Compact layout automatic, and prevent DWM previews from escaping the list viewport. | `MainWindowViewModelRefreshTests`, `SwitcherLayoutCalculatorTests`, 57 passing Release tests |
| done | Package the completed compact-overlay slice as the `v0.2.0` Windows 11 x64 Portable release. | Version metadata, changelog, 0-warning Release build, 41 tests, packaged executable smoke |
| later | Add a visible opt-out for low-level Alt+Tab capture. | Let users return to the default Windows switcher without exiting Switchboard |
| later | Add user-visible configurable-hotkey collision feedback. | Report reserved/already-registered Win32 combinations |
| later | Replace visible-state polling with Win32 event-driven catalog updates. | Preserve stable visual identity while reducing background work |
| later | Persist favorite windows. | Choose JSON or LiteDB storage |
| later | Add elevated/security-desktop activation fallback UX. | Explain foreground limitations without silent failure |
