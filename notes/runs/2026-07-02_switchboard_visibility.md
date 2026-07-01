# Run Report - Switchboard Visibility

## Finish Line

Make the WPF shell unmistakably visible on launch, keep the mock UI close to the Stitch thumbnail-grid direction, and record build/test/manual smoke evidence.

## Changes

- Registered `MainWindow` as the app shutdown owner and requested activated/topmost launch.
- Added `Esc` close handling to match the footer command hint.
- Added a process-local `windir` repair before first `Window` creation so WPF FontCache works under non-interactive launchers.
- Reworked the shell into a full-height navigation rail, separated top bar, content area, and footer.
- Improved mock window cards with clearer preview surfaces, active-window selection, monitor badges, and 3-column desktop density.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed with 0 warnings and 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe` and captured `notes/runs/2026-07-02_switchboard_visible_smoke.png`.

## Follow-Ups

- Implement Native top-level window enumeration next.
- Replace mock thumbnail surfaces with DWM thumbnails after the Native boundary is ready.
- Add distinct Compact/List templates after the real window data path is stable.
