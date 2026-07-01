# Dev Progress

## Current

- 2026-07-02: Initialized `.NET 10` WPF solution with `Switchboard.App`, `Switchboard.Core`, `Switchboard.Native`, and `Switchboard.Tests`.
- 2026-07-02: Copied Stitch export into `references/stitch/switchboard_premium_window_switcher`.
- 2026-07-02: Added mock window switcher UI using Stitch thumbnail-grid direction.
- 2026-07-02: Added Core query tests for search and favorite sorting.
- 2026-07-02: Ran the app process successfully, but user feedback says the visible UI is still insufficient.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.

## Blockers

- Runtime visual acceptance is not complete: the shell must be made clearly visible and manually smoke-tested.

## Next

- Improve launch visibility and visual polish of the mock WPF shell.
- Run app and confirm a visible Switchboard window with mock cards.
- Then implement Native window enumeration.
- Add DWM thumbnail host plan/prototype.
- Add real view-specific templates for Compact and List modes.
