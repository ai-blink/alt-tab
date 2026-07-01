# Dev Progress

## Current

- 2026-07-02: Initialized `.NET 10` WPF solution with `Switchboard.App`, `Switchboard.Core`, `Switchboard.Native`, and `Switchboard.Tests`.
- 2026-07-02: Copied Stitch export into `references/stitch/switchboard_premium_window_switcher`.
- 2026-07-02: Added mock window switcher UI using Stitch thumbnail-grid direction.
- 2026-07-02: Added Core query tests for search and favorite sorting.
- 2026-07-02: Ran the app process successfully, but user feedback says the visible UI is still insufficient.
- 2026-07-02: Started launch-visibility slice. Finish Line: app launch presents an unmistakable visible mock switcher shell, build/test pass, and a manual visual smoke artifact is recorded.
- 2026-07-02: Added startup activation basics: the app registers `MainWindow`, shows it as the shutdown owner, requests activated/topmost launch, and wires `Esc` to close.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.

## Blockers

- Runtime visual acceptance is not complete until the updated shell is manually smoke-tested.

## Next

- Improve visual polish of the mock WPF shell so it reads closer to the Stitch thumbnail-grid reference.
- Run app and confirm a visible Switchboard window with mock cards.
- Then implement Native window enumeration.
- Add DWM thumbnail host plan/prototype.
- Add real view-specific templates for Compact and List modes.
