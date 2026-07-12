# Dev Progress

## Current

- 2026-07-13: Completed the Alt+Tab, thumbnail, responsive-count, polling, and foreground-presentation stability slice.
- 2026-07-13: Alt+Tab now toggles the overlay once per gesture; plain navigation keys retain card-selection behavior.
- 2026-07-13: Low-level keyboard capture runs on a dedicated message-loop thread with repeat-key suppression and fallback-only reserved hotkey registration.
- 2026-07-13: Hiding through Alt+Tab restores the saved previous foreground window after input release.
- 2026-07-13: Non-topmost activation reaches the head of normal z-order through an immediate `TOPMOST -> NOTOPMOST` pulse without persistent topmost leakage.
- 2026-07-13: DWM thumbnails use full-source contain fitting rather than center cropping.
- 2026-07-13: Responsive row/column calculation accounts for app scale and exposes overflow beyond the visible work area.
- 2026-07-13: Window queries remain uncapped beyond 25 items, with scrolling available when all rows cannot fit.
- 2026-07-13: Unchanged catalog polls no longer notify `VisibleWindows` or rebuild WPF/DWM visuals.
- 2026-07-13: Live docs were compressed and synchronized before the stability commit; older milestones are summarized in `memory/archive-2026-07.md` and detailed under `notes/runs/`.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo --no-build`: passed, 12 tests.
- Runtime Alt+Tab smoke: 10 gestures produced `VHVHVHVHVH` with 0 visibility failures.
- Runtime foreground smoke: 0 Switchboard foreground PID failures while visible and 0 previous-PID restoration failures while hidden.
- Runtime topmost smoke with persisted `IsAlwaysOnTop=false`: 0 final `WS_EX_TOPMOST` failures.
- `git diff --check`: passed before documentation closeout.

## Blockers

- None for the completed stability slice.

## Next

- Add configurable-hotkey collision feedback or replace visible-state polling with Win32 event updates.

## Follow-Up

- FOLLOW_UP: Add a visible opt-out for low-level Alt+Tab capture before treating it as a normal default.
- FOLLOW_UP: Add elevated/security-desktop foreground failure UX.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
