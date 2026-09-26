# Dev Progress

## Current

- 2026-09-27: Enlarged the overlay top toolbar about 1.3x after user feedback (`4be2463`): header 52→64 px (`SwitcherLayoutCalculator.HeaderHeight` follows), search box 40 px / 15 pt, view segment 34 px / 13 pt, sort dropdown and icon buttons 40 px. Verified with a `PrintWindow` capture; 200% minimum height is now 784 px (within the 816 px test bound).
- 2026-09-27: Implemented the chosen overlay UX. Cards: app icon (window/class icon, falling back to the executable icon; cached per HWND) + app name capped at 5/6/7 characters (Appearance setting, default 6) that is hidden when the title starts with it; List rows show icon + full app name. Toolbar (mock 2A): search placeholder, theme segment removed from the overlay (still in Settings), sort as one dropdown button. Cards (mock 3A): 2px accent ring + 2px halo on selection (same 4px footprint), close X enlarged to 24px and shown only on the hovered or selected card; Grid/Compact captions grew 28→32 / 26→30 px to fit it.
- 2026-09-27: Alt+Tab fallback investigation. Two measured causes send Alt+Tab to the Windows switcher: (1) an elevated foreground window hides input from the standard-rights hook (UIPI); (2) the hook thread not answering within `LowLevelHooksTimeout` (500 ms here). The hook is not removed after a timeout; only presses during the stall are lost. Added: optional "Start with administrator rights at sign-in" Task Scheduler task (Behavior tab, with restart-as-admin button), single-instance mutex, hook thread `Highest` priority, EcoQoS opt-out, `SustainedLowLatency` GC, and `%AppData%\Switchboard\logs\alttab-diagnostics.log` (late hook deliveries ≥150 ms and Windows-switcher misses with the previous foreground window's elevation).
- 2026-09-27: Fixed a lost Tab key-up (e.g. to an elevated window) leaving the key filter stuck so the next Alt+Tab was silently swallowed; a Tab key-down more than 1100 ms after the previous Tab event now counts as a new press.
- 2026-09-27: UI fixes from review: selected segment/toggle uses a solid accent (white text was unreadable on the translucent accent) and stays solid on hover; the header window count is localized. The suspected overlay off-screen clipping was a DPI-unaware screenshot artifact, not a bug.
- 2026-09-27: Overlay UX mocks (app icons/labels, toolbar simplification, card display) at `notes/mocks/2026-09-27_overlay-ux/overlay-ux-mocks.html`; selection is pending the user's visual choice.
- 2026-09-27: Added Simplified Chinese (`Strings.zh-Hans.xaml`) and Japanese (`Strings.ja.xaml`) UI dictionaries and Language choices (简体中文/日本語). Auto maps any Chinese display language to Simplified Chinese. Windows get `xml:lang` for the active language so Chinese and Japanese Han glyphs render correctly.
- 2026-09-27: Added Korean/English app UI. All `MainWindow`/`SettingsWindow` strings (including the previously English-only toolbar tooltips and footer hints) and the saved-position label now come from `Localization/Strings.{ko,en}.xaml` via `DynamicResource`. A Behavior-tab Language setting (Auto/한국어/English) persists as `SelectedLanguage`; Auto follows the Windows display language and falls back to English.
- 2026-09-27: Committed the window title label scale preset (80/100/125/150%) as `048d7fc` before starting localization.
- 2026-08-19: Synchronized canonical, user, live-status, historical, reference, and template docs with the `v0.3.1` code/release baseline.
- 2026-08-18: Published the `v0.3.1` GitHub Portable release with the settings-sidebar status-card layout fix. Asset SHA-256: `B1144AE4C5F6322C4C35F0CF2BE9B1FC7E72AD078744B81FD3EF64B8D35F10C6`.
- 2026-08-17: Completed `v0.3.0`: owner-modal sidebar settings, drag-only 3×3 anchor position persistence, one-shot 9-direction movement plus saved-position return, 60/70% scale presets, and modal scale isolation.
- Earlier milestones are summarized in `memory/archive-2026-07.md`, `memory/archive-2026-08.md`, and `CHANGELOG.md`; detailed runtime evidence remains under `notes/runs/`.

## Verification

- 2026-09-27 overlay UX: build 0 warnings/errors, 110/110 tests (incl. `AppNameLabelTests`). DPI-aware Grid capture: icons render for all 24 windows, labels truncate to 6 chars with duplicates hidden (ChatGPT/Claude/Hermes/WinMux), X visible only on hovered and selected cards. Compact/List views, the sort dropdown, and the new Appearance setting are `NEEDS_USER_UI_CHECK`.
- 2026-09-27 Alt+Tab: build 0 warnings/errors, 94/94 tests. SendInput probe: suspending the app 1.5 s during Alt+Tab shows the Windows switcher and later presses work again; standard-rights Switchboard with an elevated window in front shows the Windows switcher, elevated Switchboard toggles its overlay; the diagnostic log attributed that miss to `Notepad ... elevation elevated`. Task XML registered/queried/deleted via `uac-verify` task-verify (exit 0, `RunLevel=HighestAvailable`, no residue). Single-instance: second launch exits. EcoQoS: `ControlMask=1 StateMask=0`. Actual sign-in auto-start is unverified (needs sign-out/sign-in).
- 2026-09-27 zh-Hans/ja: Debug build 0 warnings/errors, 87/87 tests pass (four-way key parity). With `SelectedLanguage` set to Japanese and then SimplifiedChinese, the app starts, stays alive, and keeps the value (settings file restored afterwards). Screens remain `NEEDS_USER_UI_CHECK`.
- 2026-09-27 localization: Debug build 0 warnings/errors, 81/81 tests pass (key parity between ko/en dictionaries, every referenced key defined, language resolution, JSON/ViewModel persistence). The new build starts and stays alive; the overlay and settings window are `NEEDS_USER_UI_CHECK`.
- 2026-08-19 documentation closeout: `git diff --check`, Debug build with 0 warnings/errors, and 57/57 tests passed.
- `dotnet build Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx -c Release --nologo`: passed for the `v0.3.1` patch, 57 tests.
- `dotnet publish` for the `v0.3.1` self-contained win-x64 single file: passed; ZIP contains `Switchboard.App.exe` and `읽어주세요.txt` and reports file version `0.3.1.0`.
- Git tag, `origin/main`, application metadata, and the public GitHub release all identify `v0.3.1`; the public asset digest matches the local package.

## Blockers

- None for the published package. Post-release UI acceptance remains incomplete.

## Next

- FOLLOW_UP: UI-check the overlay UX in Compact/List views, the sort dropdown, and Appearance → app name length; UWP windows hosted by ApplicationFrameHost may show the host icon.
- FOLLOW_UP: Turn on the sign-in auto-start in Settings, sign out/in, and confirm Switchboard runs elevated (`uac-verify` token-probe `Elevated=1`); review `alttab-diagnostics.log` after a few days of normal use to see which cause remains.
- FOLLOW_UP: UI-check localization — overlay and all three settings tabs in Korean, switching to English updates both windows and the saved-position label live, and English labels do not clip (sort segment, position remote buttons).
- FOLLOW_UP: Window title label scale 80/150% visual check across Grid/Compact/List.
- FOLLOW_UP: Verify 60/70/100/200%, Grid/Compact/List, drag persistence after restart, all 9 directional moves plus saved-position return, and settings-modal X/Esc/focus return.
- FOLLOW_UP: Check the settings sidebar status card at the target window size and verify mixed-DPI monitor fallback keeps the overlay fully visible.

## Follow-Up

- FOLLOW_UP: The logon task stores the executable path at registration; moving the Portable folder requires turning the option off and on again (no path-mismatch detection yet).
- FOLLOW_UP: Private memory is ~250–330 MB; if the diagnostics log shows late deliveries, profile allocations in the 1 s catalog refresh.
- FOLLOW_UP: zh-Hans/ja strings were machine-authored; get native-speaker review before release.
- FOLLOW_UP: Tray menu (`Open Switchboard`/`Exit`) and `AutomationProperties.Name` values remain English-only.
- FOLLOW_UP: Add a visible opt-out for low-level Alt+Tab capture before treating it as a normal default.
- FOLLOW_UP: Add elevated/security-desktop foreground failure UX.
- FOLLOW_UP: Add user-visible feedback when a protected/elevated window rejects a close request.
- FOLLOW_UP: Revisit configurable-hotkey collision feedback and Win32 event-driven catalog updates after the approved compact-overlay slice.
- IGNORE_FOR_V1: Mini dock, timeline, advanced virtual desktop management, and automatic window placement remain out of scope.
