# Run Report - Switchboard Init

## Finish Line

Initialize a buildable `.NET 10 + WPF` Switchboard project with App/Core/Native/Test boundaries, preserved Stitch design references, and a mock-data WPF shell.

## Changes

- Created `Switchboard.slnx`, `global.json`, `.gitignore`.
- Added `src/Switchboard.App`, `src/Switchboard.Core`, `src/Switchboard.Native`, and `tests/Switchboard.Tests`.
- Added Core window snapshot models and search/sort query logic.
- Added WPF shell with mock window cards based on the Stitch thumbnail-grid direction.
- Copied Stitch exports under `references/stitch/switchboard_premium_window_switcher`.
- Initialized `CLAUDE.md`, `INTENT.md`, `rules/dev-*`, templates, and init plan.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed with 0 warnings and 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- `git diff --check`: passed.
- `secret_scan.py` on docs: passed.

## User Feedback

- After app launch, user reported: "ui 아직 없네".
- Treat this as a V1 visual acceptance gap, not a build failure.

## Follow-Ups

- Make the WPF window unmistakably visible on launch and run manual visual smoke.
- Improve the mock shell visual density and mode switching before Native enumeration.
- Then implement `EnumWindows` and DWM thumbnail rendering behind `Switchboard.Native`.
