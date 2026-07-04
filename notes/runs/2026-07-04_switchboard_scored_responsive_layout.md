# Switchboard Scored Responsive Layout Run

Date: 2026-07-04

## Finish Line

Choose grid and compact overlay columns responsively from both window count and current Windows work-area size.

## Acceptance Checks

- Layout no longer uses a count-only column table.
- Candidate column counts are scored against window count, available width, available height, empty slots, right blank space, overflow, and row/column balance.
- Dense sizing policy prefers using width more aggressively than Auto.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No new pagination or grouping UI.
- No thumbnail redesign.
- No settings persistence changes.

## Changes

- Replaced `GetPreferredGridColumns()` with `ChooseBestColumns()`.
- Scored every available column count for grid and compact modes.
- Kept List mode constrained to two columns.
- Preserved the existing work-area and slot-size alignment from the prior responsive-grid fix.

## Verification

- Initial `dotnet build Switchboard.slnx --nologo` was blocked by a resident `Switchboard.App` process locking build outputs. Stopped that process and reran.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-04_switchboard_scored_responsive_layout_smoke.png`, and confirmed 29 windows lay out across the available work-area width.

## Review Budget

Used 1 implementation/review loop.

## Stop Rule

Acceptance checks passed. Future tuning can adjust score weights after more real desktop samples.
