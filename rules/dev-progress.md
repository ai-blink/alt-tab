# Dev Progress

## Current

- 2026-09-27: Added Korean/English app UI. All `MainWindow`/`SettingsWindow` strings (including the previously English-only toolbar tooltips and footer hints) and the saved-position label now come from `Localization/Strings.{ko,en}.xaml` via `DynamicResource`. A Behavior-tab Language setting (Auto/한국어/English) persists as `SelectedLanguage`; Auto follows the Windows display language and falls back to English.
- 2026-09-27: Committed the window title label scale preset (80/100/125/150%) as `048d7fc` before starting localization.
- 2026-08-19: Synchronized canonical, user, live-status, historical, reference, and template docs with the `v0.3.1` code/release baseline.
- 2026-08-18: Published the `v0.3.1` GitHub Portable release with the settings-sidebar status-card layout fix. Asset SHA-256: `B1144AE4C5F6322C4C35F0CF2BE9B1FC7E72AD078744B81FD3EF64B8D35F10C6`.
- 2026-08-17: Completed `v0.3.0`: owner-modal sidebar settings, drag-only 3×3 anchor position persistence, one-shot 9-direction movement plus saved-position return, 60/70% scale presets, and modal scale isolation.
- Earlier milestones are summarized in `memory/archive-2026-07.md`, `memory/archive-2026-08.md`, and `CHANGELOG.md`; detailed runtime evidence remains under `notes/runs/`.

## Verification

- 2026-09-27 localization: Debug build 0 warnings/errors, 81/81 tests pass (key parity between ko/en dictionaries, every referenced key defined, language resolution, JSON/ViewModel persistence). The new build starts and stays alive; the overlay and settings window are `NEEDS_USER_UI_CHECK`.
- 2026-08-19 documentation closeout: `git diff --check`, Debug build with 0 warnings/errors, and 57/57 tests passed.
- `dotnet build Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 57 tests.
- `dotnet publish` for the `v0.3.1` self-contained win-x64 single file: passed; ZIP contains `Switchboard.App.exe` and `읽어주세요.txt` and reports file version `0.3.1.0`.
- Git tag, `origin/main`, application metadata, and the public GitHub release all identify `v0.3.1`; the public asset digest matches the local package.

## Blockers

- None for the published package. Post-release UI acceptance remains incomplete.

## Next

- FOLLOW_UP: UI-check localization — overlay and all three settings tabs in Korean, switching to English updates both windows and the saved-position label live, and English labels do not clip (sort segment, position remote buttons).
- FOLLOW_UP: Window title label scale 80/150% visual check across Grid/Compact/List.
- FOLLOW_UP: Verify 60/70/100/200%, Grid/Compact/List, drag persistence after restart, all 9 directional moves plus saved-position return, and settings-modal X/Esc/focus return.
- FOLLOW_UP: Check the settings sidebar status card at the target window size and verify mixed-DPI monitor fallback keeps the overlay fully visible.

## Follow-Up

- FOLLOW_UP: Add `Strings.zh.xaml`/`Strings.ja.xaml` plus Language choices and Auto mapping to fulfil the README EN/KO/ZH/JA promise; `LocalizationTests` enforces key parity.
- FOLLOW_UP: Tray menu (`Open Switchboard`/`Exit`) and `AutomationProperties.Name` values remain English-only.
- FOLLOW_UP: Add a visible opt-out for low-level Alt+Tab capture before treating it as a normal default.
- FOLLOW_UP: Add elevated/security-desktop foreground failure UX.
- FOLLOW_UP: Add user-visible feedback when a protected/elevated window rejects a close request.
- FOLLOW_UP: Revisit configurable-hotkey collision feedback and Win32 event-driven catalog updates after the approved compact-overlay slice.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
