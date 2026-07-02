# Switchboard View Refinements Run

Date: 2026-07-03

## Finish Line

Address runtime feedback on Compact and List modes: Compact titles must not overlap DWM thumbnails, List mode should present actual two-column rows, and the overlay should expose a direct Always on top toggle.

## Acceptance Checks

- Compact mode separates the preview area from the title/caption area.
- Compact mode keeps DWM thumbnails visible without title overlap.
- List mode lays out window rows in two columns.
- List mode removes display/monitor/status columns from the visible header.
- Always on top can be toggled from the header and updates the overlay `Topmost` behavior.
- Build, tests, and runtime visual smoke pass.

## Scope Limit

- No settings persistence.
- No new large settings surface.
- No DWM source/destination crop changes.
- No foreground activation fallback UX.

## Changes

- Reworked `CompactWindowTemplate` to use separate preview and caption rows.
- Reworked `ListWindowTemplate` into a half-width row card so the existing `WrapPanel` lays rows out in two columns.
- Adjusted list sizing to prefer two columns in `MainWindow.xaml.cs`.
- Updated vertical keyboard movement to use the current grid column count in all modes.
- Added an `Always on top` header toggle backed by `MainWindowViewModel.IsAlwaysOnTop`.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected Compact and List modes through UI Automation, and captured:
  - `notes/runs/2026-07-03_switchboard_view_refinements_compact_smoke.png`
  - `notes/runs/2026-07-03_switchboard_view_refinements_list_two_column_smoke.png`

## Review Budget

Used 2 implementation/review loops. The second loop changed List mode from two information columns inside one row to actual two-column row layout.

## Stop Rule

Acceptance checks passed. Persistence and activation fallback remain follow-ups.
