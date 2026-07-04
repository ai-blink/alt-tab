# Switchboard App Scale Run

Date: 2026-07-05

## Finish Line

Add whole-overlay app scale presets so the user can scale the switcher UI and native overlay window together at 50%, 70%, 90%, 100%, or 120%.

## Acceptance Checks

- Settings popup exposes 50%, 70%, 90%, 100%, and 120% app scale choices separately from overlay opacity.
- The root shell and settings popup scale together.
- The native WPF window bounds are resized from the last logical layout size instead of changing only thumbnail sizes.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No settings persistence yet.
- No new settings page or sidebar.
- No changes to thumbnail scale, opacity, or hotkey registration behavior.

## Changes

- Added `OverlayScalePreset` and `SelectedOverlayScalePreset` state.
- Added `AppScale` binding and applied it to the root overlay and settings popup.
- Split layout minimums from WPF `MinWidth` / `MinHeight` so 50% and 70% can shrink the real native window.
- Preserved the last logical layout width/height and apply the selected app scale to those bounds when only the app scale changes.

## Verification

- Initial `dotnet build Switchboard.slnx --nologo` was blocked by a resident `Switchboard.App` process locking build outputs. Stopped that process and reran.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened settings through UI Automation, selected `App scale 50%`, and captured `notes/runs/2026-07-05_switchboard_app_scale_smoke.png`.
- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors after adding 100% and 120% app scale presets.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests after adding 100% and 120% app scale presets.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, opened settings through UI Automation, and confirmed `App scale 120%` is visible and selectable.

## Review Budget

Used 1 implementation/review loop.

## Stop Rule

Acceptance checks passed. Future work should persist the selected app scale with the rest of overlay settings.
