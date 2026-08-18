# Dev Progress

## Current

- 2026-08-18: Prepared the `v0.3.1` self-contained win-x64 Portable ZIP for the settings-sidebar status-card layout fix. SHA-256: `B1144AE4C5F6322C4C35F0CF2BE9B1FC7E72AD078744B81FD3EF64B8D35F10C6`.
- 2026-08-17: Published the `v0.3.0` Windows 11 x64 Portable GitHub release with `Switchboard-v0.3.0-win-x64-Portable.zip`.
- 2026-08-17: Prepared the `v0.3.0` self-contained win-x64 Portable ZIP with the settings-window, persistent-position, 60/70% scale, and modal clipping fixes. SHA-256: `A3A84823263E04267120D5EA20FFA3F505F503DCC1C2934C0DA08C20F9640A73`.
- 2026-08-10: Approved the blue-violet four-pane Switchboard icon, generated a transparent-corner multi-resolution `.ico`, and replaced the procedural tray/window icon with the shared application asset for the `v0.2.1` patch release.
- 2026-08-10: Built the self-contained `v0.2.1` win-x64 single executable and packaged `Switchboard-v0.2.1-win-x64-Portable.zip` (SHA-256 `25EA0A84275CC320934001669DD087305ABE00454E8D4D5C896520C19333DCBB`).
- 2026-08-10: Reviewed all 42 commits through `7841f76` and scoped the `v0.2.0` release delta to the six commits after `v0.1.1`; superseded independent size and manual row/column experiments are excluded from the final patch notes.
- 2026-08-10: Updated application version metadata, user documentation, and changelog for the `v0.2.0` Windows 11 x64 Portable release.
- 2026-08-10: Built the self-contained `v0.2.0` win-x64 single executable, packaged it with the Korean readme, verified its embedded file version and ZIP entries, and completed a responsive-process smoke test.
- 2026-08-10: A runtime screenshot showed WPF wrapping a calculated four-column layout into three columns. The items panel now renders the calculator's exact column count with `UniformGrid`, with additional horizontal viewport safety for scrollbar, border, and DPI rounding.
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

- `dotnet build Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 57 tests.
- `dotnet publish` for the `v0.3.1` self-contained win-x64 single file: passed; ZIP contains `Switchboard.App.exe` and `읽어주세요.txt` and reports file version `0.3.1.0`.
- GitHub Release `v0.3.0`: published with one uploaded ZIP asset whose GitHub SHA-256 digest matches the local package.
- `dotnet build Switchboard.slnx -c Release --nologo`: passed for the `v0.3.0` release, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed for the `v0.3.0` release, 57 tests.
- `dotnet publish` for the `v0.3.0` self-contained win-x64 single file: passed; ZIP contains `Switchboard.App.exe` and `읽어주세요.txt`.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet build Switchboard.slnx -c Release --nologo`: passed for the `v0.2.1` icon patch, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed for the `v0.2.1` icon patch, 41 tests.
- `dotnet publish` for the `v0.2.1` self-contained win-x64 single file: passed; package contains `Switchboard.App.exe` and `읽어주세요.txt`.
- `dotnet build Switchboard.slnx -c Release --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed, 41 tests.
- `dotnet publish` for the `v0.2.0` self-contained win-x64 single file: passed; package contains `Switchboard.App.exe` and `읽어주세요.txt`.
- Packaged `Switchboard.App.exe`: file version `0.2.0.0`; process stayed alive and responsive for the smoke interval, then was stopped.
- Focused scale/layout/settings regressions are included in the passing Release suite.
- `notes/mocks/2026-08-09_compact-overlay-controls/compact-overlay-controls-mocks.html`: inline JavaScript syntax and required control checks passed.
- Runtime Alt+Tab smoke: 10 gestures produced `VHVHVHVHVH` with 0 visibility failures.
- Runtime foreground smoke: 0 Switchboard foreground PID failures while visible and 0 previous-PID restoration failures while hidden.
- Runtime topmost smoke with persisted `IsAlwaysOnTop=false`: 0 final `WS_EX_TOPMOST` failures.
- `git diff --check`: passed before documentation closeout.

## Blockers

- None for the completed stability slice.

## Next

- FOLLOW_UP: Visually verify 80% compact recovery, 125% plus 1.2x thumbnail reflow, 200% compact bounds, DWM scrolling, and the position segments.
- FOLLOW_UP: Visually check the final icon in the Windows taskbar, tray overflow area, and executable file properties on an installed Portable build.

## Follow-Up

- FOLLOW_UP: Add a visible opt-out for low-level Alt+Tab capture before treating it as a normal default.
- FOLLOW_UP: Add elevated/security-desktop foreground failure UX.
- FOLLOW_UP: Add user-visible feedback when a protected/elevated window rejects a close request.
- FOLLOW_UP: Revisit configurable-hotkey collision feedback and Win32 event-driven catalog updates after the approved compact-overlay slice.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
