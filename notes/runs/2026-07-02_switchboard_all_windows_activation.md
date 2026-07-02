# Switchboard All Windows And Double-Click Activation Run

Date: 2026-07-02

## Finish Line

The compact overlay keeps its fixed shell size while showing all filtered windows, scrolling within Grid/Compact/List modes, improving DWM thumbnail fit without aggressive cover cropping, and activating a selected real window by either Enter or double-click.

## Acceptance Checks

- `VisibleWindows` no longer caps the result set at 6 windows.
- Grid, Compact, and List modes expose more windows through in-overlay vertical scrolling.
- Grid and Compact retain dense 3-column layouts when a vertical scrollbar is present.
- DWM thumbnails preserve most of the source while reducing empty contain letterbox space.
- Double-clicking a card or row uses the same foreground activation request as Enter and closes the overlay only on success.
- Build, tests, and runtime visual smoke pass.

## Scope Limit

- No foreground activation fallback UX.
- No settings persistence.
- No Alt+Tab interception or OS replacement behavior.
- No broad visual redesign beyond thumbnail fit, letterbox treatment, and fixed-shell scrolling.

## Changes

- Removed the `Take(6)` cap from `MainWindowViewModel.VisibleWindows`.
- Enabled vertical scrolling on the switcher `ListBox` so all modes can reveal more windows without resizing the overlay.
- Added a shared activation helper and `MouseDoubleClick` handler for cards/rows.
- Added a DWM thumbnail source rect with a maximum 8% soft crop before fitting, avoiding the earlier cover/crop behavior while reducing contain whitespace.
- Switched preview letterbox backgrounds to the overlay search surface with a muted app-color tint.
- Narrowed grid/compact cards slightly so 3 columns still fit when the scrollbar is visible.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected each view mode through UI Automation, and captured:
  - `notes/runs/2026-07-02_switchboard_all_windows_grid_smoke.png`
  - `notes/runs/2026-07-02_switchboard_all_windows_compact_smoke.png`
  - `notes/runs/2026-07-02_switchboard_all_windows_list_smoke.png`
- Visual check confirmed `12 windows` on the current desktop, fixed overlay size, visible scrollbar, 3-column Grid/Compact layouts, List scrolling, and nonblank DWM thumbnails.

## Review Budget

Used 2 implementation/review loops. The second loop reduced card width to restore the intended 3-column Grid/Compact layout after enabling the scrollbar.

## Stop Rule

Acceptance checks passed. Scrollbar styling and foreground-lock fallback UX remain follow-ups.
