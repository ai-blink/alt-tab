# Switchboard Settings Panel Run

Date: 2026-07-03

## Finish Line

Add a compact in-overlay settings button and popover without introducing a new page or sidebar. The popover exposes thumbnail scale presets, sizing policy, and default view mode candidates, and thumbnail scale can enlarge live DWM previews without increasing crop aggressiveness or breaking title areas.

## Acceptance Checks

- Header has a small settings button that matches the existing compact overlay chrome.
- Settings opens inside the overlay as a small popover.
- Popover includes thumbnail scale options for `1.0x`, `1.1x`, and `1.2x`.
- Popover includes a grid/auto sizing policy option.
- Popover includes default view mode candidates for Grid, Compact, and List.
- Thumbnail scale changes card/preview dimensions while the DWM source rect soft contain/crop logic remains unchanged.
- Grid, Compact, and List render after settings changes.
- Build, tests, and runtime visual smoke pass.

## Scope Limit

- No settings persistence yet.
- No new large settings page or sidebar.
- No new DWM crop algorithm.
- No foreground activation fallback UX.
- No Alt+Tab interception or OS replacement behavior.

## Changes

- Added `ThumbnailScalePreset` and `SwitcherSizingPolicy` Core model enums for later settings persistence.
- Added settings state to `MainWindowViewModel`: thumbnail scale preset, sizing policy, default view mode candidate, and open/closed popover state.
- Bound Grid, Compact, and List template dimensions to ViewModel card/preview size properties.
- Updated shell layout calculation to use scaled card sizes and a dense sizing policy option.
- Added a gear settings button and compact popover in `MainWindow.xaml`.
- Kept DWM thumbnail source/destination soft contain behavior unchanged; scale increases available preview/card size instead.
- Narrowed custom chrome dragging to the title area so header buttons and segmented controls receive clicks reliably.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened settings via UI Automation, selected `1.2x`, and selected Grid, Compact, and List. Captured:
  - `notes/runs/2026-07-03_switchboard_settings_grid_smoke.png`
  - `notes/runs/2026-07-03_switchboard_settings_panel_smoke.png`
  - `notes/runs/2026-07-03_switchboard_settings_scale_1_2_smoke.png`
  - `notes/runs/2026-07-03_switchboard_settings_compact_smoke.png`
  - `notes/runs/2026-07-03_switchboard_settings_list_smoke.png`
  - `notes/runs/2026-07-03_switchboard_settings_grid_restored_smoke.png`

## Review Budget

Used 2 implementation/review loops. The second loop fixed header chrome drag hit testing so the settings button could be clicked reliably.

## Stop Rule

Acceptance checks passed. Persistence and activation fallback remain follow-ups.
