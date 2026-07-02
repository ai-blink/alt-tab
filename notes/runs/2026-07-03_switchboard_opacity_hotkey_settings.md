# Switchboard Opacity And Hotkey Settings Run

Date: 2026-07-03

## Finish Line

Add compact settings for overlay opacity and switcher hotkey candidates without implementing global hotkey registration or settings persistence.

## Acceptance Checks

- Settings popover exposes overlay opacity options: `80%`, `90%`, and `100%`.
- Opacity changes scale shell/header/card/search/segment background alpha without changing the existing Transparent/Dark/Light appearance modes.
- Settings popover exposes hotkey candidates: `Alt+Space`, `Ctrl+Space`, and `Ctrl+Alt+Tab`.
- State is represented with Core model enums so later settings persistence can attach to the same surface.
- Grid, Compact, and List views still render.
- Build, tests, and runtime visual smoke pass.

## Scope Limit

- No settings persistence.
- No global hotkey registration.
- No foreground activation fallback UX.
- No large settings page or sidebar.

## Changes

- Added `OverlayOpacityPreset` and `SwitcherHotkeyPreset` Core model enums.
- Added `SelectedOverlayOpacityPreset` and `SelectedHotkeyPreset` state to `MainWindowViewModel`.
- Scaled the overlay background alpha through a shared opacity factor while leaving text/accent brushes fully readable.
- Added compact settings rows for opacity and hotkey candidates.
- Added a `PopoverBackground` brush for better settings readability.
- Added `DwmThumbnailPreview.IsPreviewVisible` so DWM thumbnails unregister while the settings popover is open; this avoids native DWM thumbnails drawing above the WPF popover.

## Verification

- Initial `dotnet build Switchboard.slnx --nologo` was blocked by an existing `Switchboard.App` process locking build outputs. Stopped that process and reran.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened settings through UI Automation, selected `80%`, selected `Ctrl+Space`, and selected Grid, Compact, and List. Captured:
  - `notes/runs/2026-07-03_switchboard_opacity_hotkey_grid_smoke.png`
  - `notes/runs/2026-07-03_switchboard_opacity_hotkey_settings_open_smoke.png`
  - `notes/runs/2026-07-03_switchboard_opacity_80_smoke.png`
  - `notes/runs/2026-07-03_switchboard_hotkey_ctrl_space_smoke.png`
  - `notes/runs/2026-07-03_switchboard_opacity_hotkey_compact_smoke.png`
  - `notes/runs/2026-07-03_switchboard_opacity_hotkey_list_smoke.png`
  - `notes/runs/2026-07-03_switchboard_opacity_hotkey_grid_restored_smoke.png`

## Review Budget

Used 2 implementation/review loops. The second loop fixed DWM thumbnail z-order over the settings popover.

## Stop Rule

Acceptance checks passed. Global hotkey registration and settings persistence remain follow-ups.
