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

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_visible_smoke.png`, and confirmed visible nav, top bar, 3-column mock cards, active selection ring, and footer.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_compact_transparent_smoke.png`, and confirmed compact transparent overlay layout with 3x2 cards and appearance mode controls.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-02_switchboard_dwm_thumbnail_smoke.png`, and confirmed real window enumeration plus DWM thumbnail previews.

## Blockers

- None for the current mock shell visibility slice.

## Next

- Add keyboard selection/focus behavior: Tab/arrows cycle, Enter activates selected window.
- Add real view-specific templates for Compact and List modes after the overlay interaction model is stable.

## Follow-Up

- FOLLOW_UP: Improve thumbnail polish for transparent mode once real activation/hotkey behavior is in place.
- FOLLOW_UP: Persist the selected appearance mode once settings persistence exists.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
