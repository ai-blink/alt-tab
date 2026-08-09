# Dev Progress

## Current

- 2026-08-10: Replaced the prototype's compact-only `0.7` multiplier with one unified `80/100/125/150/200%` overlay scale; legacy JSON scale names normalize to the nearest new safe preset.
- 2026-08-10: Compact bounds now expand beyond the 65% target when needed for a complete card row, calculated columns are bound to the actual WrapPanel width, and off-viewport DWM thumbnails are hidden.
- 2026-08-10: Replaced the compact-position ComboBox with the shared segmented-radio visual language and equalized fixed-option setting rows.
- 2026-08-10: User superseded the earlier independent window-size/UI-size Option 1. Manual maximum rows/columns and separate size controls remain out of scope.
- 2026-08-09: User approved compact-overlay Option 1: selectable position, five window-size levels, five independent UI-size levels, and automatic card layout.
- 2026-08-09: Added an interactive three-option HTML mock; Option 2 was corrected to remove its conflicting window-size control before Option 1 was selected.
- 2026-08-09: Added a custom Switchboard icon for the tray and taskbar window; the Release app launched successfully.
- 2026-08-09: Added a persisted compact-overlay prototype and position calculation with tests. It is a baseline only: its combined 70% presentation scale must be replaced by the approved Option 1 behavior.
- 2026-07-20: Added always-visible close controls to Grid/Compact/List cards and routed standard `WM_CLOSE` requests through a dedicated `IWindowCloser` boundary.
- 2026-07-20: Moved Grid/Compact close controls outside the DWM thumbnail destination to prevent render-order occlusion.
- 2026-07-20: Prepared version metadata and user documentation for the `v0.1.1` Portable release.
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
- `dotnet build Switchboard.slnx -c Release --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed, 40 tests.
- Focused scale/layout/settings regressions are included in the passing Release suite.
- `notes/mocks/2026-08-09_compact-overlay-controls/compact-overlay-controls-mocks.html`: inline JavaScript syntax and required control checks passed.
- Runtime Alt+Tab smoke: 10 gestures produced `VHVHVHVHVH` with 0 visibility failures.
- Runtime foreground smoke: 0 Switchboard foreground PID failures while visible and 0 previous-PID restoration failures while hidden.
- Runtime topmost smoke with persisted `IsAlwaysOnTop=false`: 0 final `WS_EX_TOPMOST` failures.
- `git diff --check`: passed before documentation closeout.

## Blockers

- None for the completed stability slice.

## Next

- Run the new Release build on request and visually verify 80% compact recovery, 125% plus 1.2x thumbnail reflow, 200% compact bounds, DWM scrolling, and the position segments.

## Follow-Up

- FOLLOW_UP: Add a visible opt-out for low-level Alt+Tab capture before treating it as a normal default.
- FOLLOW_UP: Add elevated/security-desktop foreground failure UX.
- FOLLOW_UP: Add user-visible feedback when a protected/elevated window rejects a close request.
- FOLLOW_UP: Revisit configurable-hotkey collision feedback and Win32 event-driven catalog updates after the approved compact-overlay slice.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
