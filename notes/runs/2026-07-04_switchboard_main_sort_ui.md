# Switchboard Main Sort UI Run

Date: 2026-07-04

## Finish Line

Expose the existing window sort modes directly in the main overlay header.

## Acceptance Checks

- Main header shows sort controls without opening settings.
- Sort controls include `최근`, `앱`, `모니터`, `제목`, and `즐겨찾기`.
- Controls bind to the existing `SelectedSortMode` and reorder `VisibleWindows`.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No settings persistence.
- No new sort algorithms.
- No large layout redesign.

## Changes

- Replaced the passive `SelectedSortMode` text in the header with segmented sort buttons.
- Bound each sort button to `SelectedSortMode` through the existing `EnumEqualsConverter`.
- Kept the existing compact Windows overlay chrome and mode control style.

## Verification

- Initial `dotnet build Switchboard.slnx --nologo` was blocked by a resident `Switchboard.App` process locking build outputs. Stopped that process and reran.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected `앱` sort through UI Automation, and captured `notes/runs/2026-07-04_switchboard_main_sort_ui_smoke.png`.

## Review Budget

Used 2 implementation/review loops. The second loop replaced a default WPF ComboBox with the existing segmented overlay style.

## Stop Rule

Acceptance checks passed. Settings persistence remains a separate follow-up.
