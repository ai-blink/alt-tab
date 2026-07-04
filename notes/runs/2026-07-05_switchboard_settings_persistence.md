# Switchboard Settings Persistence Run

Date: 2026-07-05

## Finish Line

Persist user-facing switcher settings automatically and close the settings popup when the user clicks outside the panel.

## Acceptance Checks

- Settings are saved to a user-local JSON file when changed.
- App startup restores saved view, sort, appearance, opacity, app scale, thumbnail scale, sizing policy, default view, hotkey, and always-on-top values.
- The settings popup stays open for internal interaction but closes on outside click.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No favorite window persistence.
- No settings import/export UI.
- No hotkey collision feedback UI.

## Changes

- Added `UserSettings`, `IUserSettingsStore`, and `JsonUserSettingsStore`.
- Registered the settings store in the app DI container.
- Loaded settings in `MainWindowViewModel` before external property subscriptions are attached.
- Saved persisted settings whenever a persisted setting property changes.
- Changed the settings popup to `StaysOpen="False"` so outside clicks close it.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected `App scale 120%` and `Sort by title`, confirmed `%AppData%\Switchboard\settings.json` stores the values, restarted the app, confirmed both values restore, and confirmed an outside click closes the settings popup.
- The temporary smoke settings file was removed after verification so the user's local app settings were not left changed by the test.

## Review Budget

Used 1 implementation/review loop.

## Stop Rule

Acceptance checks passed. Future work can add collision feedback for unavailable global hotkeys.
