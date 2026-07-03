# Dev Progress

## Current

- 2026-07-02: Initialized `.NET 10` WPF solution with `Switchboard.App`, `Switchboard.Core`, `Switchboard.Native`, and `Switchboard.Tests`.
- 2026-07-02: Copied Stitch export into `references/stitch/switchboard_premium_window_switcher`.
- 2026-07-02: Added mock window switcher UI using Stitch thumbnail-grid direction.
- 2026-07-02: Added Core query tests for search and favorite sorting.
- 2026-07-02: Ran the app process successfully, but user feedback says the visible UI is still insufficient.
- 2026-07-02: Started launch-visibility slice. Finish Line: app launch presents an unmistakable visible mock switcher shell, build/test pass, and a manual visual smoke artifact is recorded.
- 2026-07-02: Added startup activation basics: the app registers `MainWindow`, shows it as the shutdown owner, requests activated/topmost launch, and wires `Esc` to close.
- 2026-07-02: Reworked the mock shell toward the Stitch thumbnail-grid reference: full-height navigation, separated top bar/content/footer, clearer card previews, icon controls, and active-window selection by default.
- 2026-07-02: Fixed a non-interactive launcher crash where WPF FontCache needs process-local `windir` before the first `Window` is created.
- 2026-07-02: Completed launch visibility visual smoke. Artifact: `notes/runs/2026-07-02_switchboard_visible_smoke.png`.
- 2026-07-02: Replaced the oversized management-style shell with a compact transparent overlay: no sidebar, no large footer, 3x2 mock window cards, top search, draggable custom chrome, and in-overlay `투명` / `다크` / `라이트` appearance modes.
- 2026-07-02: Connected the overlay to real Win32 top-level window enumeration and DWM thumbnails. The compact overlay now shows live window titles/process names and DWM-rendered previews for the first 6 windows.
- 2026-07-02: Rebalanced cards so thumbnails are the primary surface. Titles are now a single-line bottom caption instead of taking half the card.
- 2026-07-02: Doubled the thumbnail-first card height while preserving the 3x2 switcher layout. The DWM preview area is now the dominant surface, with titles kept as compact captions.
- 2026-07-02: Completed keyboard selection and foreground activation slice. Tab/Shift+Tab and arrow keys move the selected card, Enter requests foreground activation for the selected real window, and Esc still closes the overlay.
- 2026-07-02: Completed view mode templates slice. The header now exposes `격자` / `압축` / `목록` view modes, each backed by a distinct WPF `DataTemplate` while keeping the same compact overlay size and real DWM thumbnail source.
- 2026-07-02: Completed all-windows and double-click activation slice. The overlay no longer caps visible windows at 6, grid/compact/list modes scroll inside the fixed shell, cards/rows double-click through the existing foreground activation path, and DWM thumbnails use a soft contain crop plus toned letterbox background instead of aggressive cover cropping.
- 2026-07-03: Completed count-sized overlay slice. The overlay now starts with manual centered bounds, grows to fit the current filtered window count when possible, hides the in-overlay scrollbar, and keeps the grid column count synchronized with the calculated shell size.
- 2026-07-03: Completed compact settings panel slice. The header now has a small settings button that opens an in-overlay popover for thumbnail scale presets (`1.0x`, `1.1x`, `1.2x`), sizing policy (`Auto`, `Dense`), and default view mode candidates. Thumbnail scale enlarges card/preview dimensions while keeping the existing DWM soft contain/crop logic.
- 2026-07-03: Completed view refinement slice. Compact mode now separates thumbnail and caption rows so titles do not overlap previews, List mode renders real two-column rows, and the header includes an `Always on top` toggle bound to overlay topmost behavior.
- 2026-07-03: Completed opacity and hotkey settings slice. The settings popover now includes overlay opacity presets (`25%`, `50%`, `75%`, `90%`) and three hotkey combo boxes for first modifier, second modifier, and key, backed by Core model enums for later persistence. DWM thumbnails are temporarily unregistered while settings is open so native thumbnail composition does not draw above the popover.
- 2026-07-03: Added tray residency groundwork for global hotkeys. Closing the overlay now hides it instead of shutting down the process, the app owns a tray icon with Open and Exit actions, and only tray Exit requests application shutdown.
- 2026-07-03: Expanded the hotkey key combo box from a small test set to `Space`, `Tab`, `Enter`, `A-Z`, `0-9`, and `F1-F12`, with display labels separated from enum storage values.
- 2026-07-03: Connected selected hotkey settings to Win32 `RegisterHotKey`. The app now registers the current modifier/key combo on startup, refreshes registration when combo boxes change, and handles `WM_HOTKEY` by showing the overlay while the process remains resident in the tray.
- 2026-07-04: Fixed settings-open thumbnail regression. The settings surface now uses a WPF `Popup` anchored to the settings button, so the menu renders above native DWM thumbnails without unregistering thumbnail previews.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after keyboard activation changes.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after adding selection helper coverage.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_visible_smoke.png`, and confirmed visible nav, top bar, 3-column mock cards, active selection ring, and footer.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_compact_transparent_smoke.png`, and confirmed compact transparent overlay layout with 3x2 cards and appearance mode controls.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_dwm_thumbnail_smoke.png`, and confirmed real window enumeration plus DWM thumbnail previews.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_thumbnail_first_smoke.png`, and confirmed thumbnail-first cards with compact bottom captions.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_thumbnail_double_height_smoke.png`, and confirmed roughly doubled thumbnail height with compact captions.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_keyboard_activation_smoke.png`, and confirmed the compact overlay still opens with real windows and visible selected-card focus.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after view mode template changes.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after view mode template changes.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected `격자` / `압축` / `목록` via UI Automation, captured `notes/runs/2026-07-02_switchboard_view_modes_grid_smoke.png`, `notes/runs/2026-07-02_switchboard_view_modes_compact_smoke.png`, and `notes/runs/2026-07-02_switchboard_view_modes_list_smoke.png`, and confirmed all three templates render without increasing the overlay.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after all-windows/double-click/thumbnail changes. Initial attempt was blocked by a previous running `Switchboard.App` process locking the exe; stopped that process and reran successfully.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after all-windows/double-click/thumbnail changes.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected `격자` / `압축` / `목록` via UI Automation, captured `notes/runs/2026-07-02_switchboard_all_windows_grid_smoke.png`, `notes/runs/2026-07-02_switchboard_all_windows_compact_smoke.png`, and `notes/runs/2026-07-02_switchboard_all_windows_list_smoke.png`, and confirmed the overlay showed 12 windows on the current desktop with fixed shell size, scrolling, live DWM thumbnails, and 3-column grid/compact layouts.
- Manual visual smoke: captured `notes/runs/2026-07-02_switchboard_count_sized_desktop_smoke.png` after `0a15adf` and confirmed the overlay expands with the available desktop bounds instead of depending on a visible scrollbar.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after settings panel and thumbnail scale changes.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after settings panel and thumbnail scale changes.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened the settings panel and selected `1.2x` via UI Automation, then selected `격자` / `압축` / `목록`. Captured `notes/runs/2026-07-03_switchboard_settings_panel_smoke.png`, `notes/runs/2026-07-03_switchboard_settings_scale_1_2_smoke.png`, `notes/runs/2026-07-03_switchboard_settings_compact_smoke.png`, and `notes/runs/2026-07-03_switchboard_settings_list_smoke.png`.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after compact/list/topmost view refinements.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after compact/list/topmost view refinements.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected Compact and List modes via UI Automation, and captured `notes/runs/2026-07-03_switchboard_view_refinements_compact_smoke.png` plus `notes/runs/2026-07-03_switchboard_view_refinements_list_two_column_smoke.png`.
- Initial `dotnet build Switchboard.slnx --nologo` after opacity/hotkey changes was blocked by a previous `Switchboard.App` process locking build outputs; stopped that process and reran successfully.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after opacity/hotkey settings changes.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after opacity/hotkey settings changes.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened settings via UI Automation, selected `80%`, selected `Ctrl+Space`, selected Grid/Compact/List, and captured `notes/runs/2026-07-03_switchboard_opacity_hotkey_settings_open_smoke.png`, `notes/runs/2026-07-03_switchboard_opacity_80_smoke.png`, `notes/runs/2026-07-03_switchboard_hotkey_ctrl_space_smoke.png`, `notes/runs/2026-07-03_switchboard_opacity_hotkey_compact_smoke.png`, and `notes/runs/2026-07-03_switchboard_opacity_hotkey_list_smoke.png`.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after tray residency changes.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after tray residency changes.
- Runtime smoke: launched `Switchboard.App.exe`, confirmed the process starts and remains resident for tray/hotkey groundwork, then stopped the test process.
- Initial `dotnet build Switchboard.slnx --nologo` after expanding hotkey keys was blocked by a resident `Switchboard.App` process locking outputs; stopped that process and reran successfully.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after expanding hotkey keys.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after expanding hotkey keys.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after global hotkey registration.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after global hotkey registration.
- Initial `dotnet build Switchboard.slnx --nologo` after the settings popup fix was blocked by a resident `Switchboard.App` process locking build outputs; stopped that process and reran successfully.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after the settings popup fix.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after the settings popup fix.
- Runtime smoke: launched `Switchboard.App.exe`, opened settings through UI Automation, captured `notes/runs/2026-07-04_switchboard_settings_popup_thumbnails_smoke.png`, and confirmed DWM thumbnails remain visible while the settings menu is open.

## Blockers

- None for the current settings panel slice.

## Next

- Begin settings persistence for hotkey and overlay settings, or design the activation fallback UX for Windows foreground-lock failures.

## Follow-Up

- FOLLOW_UP: Improve thumbnail polish for transparent mode once real activation/hotkey behavior is in place.
- FOLLOW_UP: Persist the selected appearance mode once settings persistence exists.
- FOLLOW_UP: Add a user-visible fallback for Windows foreground-lock cases where `SetForegroundWindow` refuses activation even after the overlay requests it.
- FOLLOW_UP: Persist overlay settings once the settings surface stabilizes.
- FOLLOW_UP: Add user-visible feedback when a selected global hotkey cannot be registered because it is reserved or already in use.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
