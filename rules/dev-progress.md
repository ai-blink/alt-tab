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

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_visible_smoke.png`, and confirmed visible nav, top bar, 3-column mock cards, active selection ring, and footer.

## Blockers

- None for the current mock shell visibility slice.

## Next

- Then implement Native window enumeration.
- Add DWM thumbnail host plan/prototype.
- Add real view-specific templates for Compact and List modes.

## Follow-Up

- FOLLOW_UP: Replace mock thumbnail surfaces with real DWM thumbnails after Native enumeration is available.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
