# Switchboard View Mode Templates Run

Date: 2026-07-02

## Finish Line

The compact overlay exposes Grid, Compact, and List view modes with distinct WPF templates, while preserving the thumbnail-first direction, real DWM preview path, and existing keyboard activation flow.

## Acceptance Checks

- `격자`, `압축`, and `목록` controls are visible in the overlay header.
- Each mode renders a distinct layout without increasing the `860x540` overlay.
- DWM thumbnails remain the primary visual signal in all modes.
- Tab, arrow selection, Enter activation, and Esc close remain wired.
- Build, tests, and visual smoke pass.

## Scope Limit

- No settings persistence.
- No foreground activation fallback UX.
- No larger shell, sidebar, timeline, mini dock, or workspace management.

## Changes

- Moved the existing grid card into `GridWindowTemplate`.
- Added `CompactWindowTemplate` with small DWM thumbnails and two-row metadata.
- Added `ListWindowTemplate` with dense rows, preview thumbnails, app/title/monitor metadata, and active status.
- Added a header segmented control for `격자` / `압축` / `목록`.
- Kept list-mode arrow navigation row-oriented by using a one-item vertical selection step only when `SelectedViewMode` is `List`.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected each view mode through UI Automation, and captured:
  - `notes/runs/2026-07-02_switchboard_view_modes_grid_smoke.png`
  - `notes/runs/2026-07-02_switchboard_view_modes_compact_smoke.png`
  - `notes/runs/2026-07-02_switchboard_view_modes_list_smoke.png`

## Review Budget

- Used 1 implementation/review loop. No blocker follow-up loop was needed.

## Stop Rule

Acceptance checks passed; adjacent activation fallback and persistence work remain follow-ups.
