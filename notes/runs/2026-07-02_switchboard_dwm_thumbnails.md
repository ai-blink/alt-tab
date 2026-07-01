# Run Report - DWM Thumbnails

## Finish Line

Connect the compact overlay to real top-level Windows windows and show actual DWM thumbnail previews instead of mock-only preview panels.

## Changes

- Implemented `Win32NativeWindowProvider` with `EnumWindows`, title/process extraction, basic Alt+Tab-style filtering, monitor labels, and active-window detection.
- Switched app DI from `DemoWindowCatalog` to the native provider.
- Added `DwmThumbnailPreview`, which registers DWM thumbnails against the main top-level overlay window and updates the destination rectangle from WPF layout.
- Added real DWM previews to the card preview region while keeping a subtle mock fallback behind them.
- Limited the compact overlay to the first 6 visible windows so the 3x2 layout and header count match.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed with 0 warnings and 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe` and captured `notes/runs/2026-07-02_switchboard_dwm_thumbnail_smoke.png`.

## Follow-Ups

- Add `Tab`/arrow key selection and `Enter` foreground activation.
- Tune thumbnail presentation further after real activation/hotkey behavior is in place.
