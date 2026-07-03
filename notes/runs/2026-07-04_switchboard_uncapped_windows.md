# Switchboard Uncapped Native Windows Run

Date: 2026-07-04

## Finish Line

Remove the remaining 12-window cap so the switcher can receive every candidate top-level window from the native Win32 provider.

## Acceptance Checks

- Native window enumeration no longer truncates the result list to 12.
- Existing sorting keeps the active window first.
- Overlay sizing and grid column logic still handle larger window counts.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No filtering policy changes.
- No virtual desktop management.
- No new pagination or grouping UI.

## Changes

- Removed `.Take(12)` from `Win32NativeWindowProvider.GetTopLevelWindows()`.
- Kept the existing active-window-first ordering.
- Updated progress and roadmap docs.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, captured `notes/runs/2026-07-04_switchboard_uncapped_windows_smoke.png`, and confirmed the header shows `29 windows`.

## Review Budget

Used 1 implementation/review loop.

## Stop Rule

Acceptance checks passed. Further large-window-count UX improvements such as grouping or pagination remain separate follow-up work.
