# Changelog

All notable project changes are recorded here by version.

## Release notes and languages

This file is the canonical, detailed English changelog. For every GitHub Release, use the [multilingual release-notes template](doc/templates/release-notes.md).

- **English:** include the complete user-facing change list.
- **Korean, Simplified Chinese, and Japanese:** translate the user-impacting summary for ordinary releases.
- **Security notices, data-loss risks, breaking changes, installation changes, and required user actions:** provide the complete information in all four languages.
- **Internal refactors, test-only work, and developer-tooling changes:** may remain in the English changelog when they do not affect users.

## [0.4.0] - 2026-09-27

A feature release that makes the app multilingual, keeps Alt+Tab on Switchboard more reliably, and refreshes the overlay cards and toolbar.

### Added

- English, Simplified Chinese, and Japanese app UI alongside Korean, with a Language setting (Auto, 한국어, English, 简体中文, 日本語) in the Behavior tab. Auto follows the Windows display language (any Chinese variant uses Simplified Chinese) and uses English for unsupported languages; switching applies immediately without restarting.
- Window title size setting (80/100/125/150%) that scales window titles and app labels in Grid, Compact, and List views.
- Optional "Start with administrator rights at sign-in" setting (Behavior tab). Windows hides keyboard input for administrator windows such as Task Manager from standard apps, so Alt+Tab over them used to open the Windows switcher; running elevated through a Task Scheduler sign-in task keeps Switchboard in charge. Includes a "Restart as administrator now" button and a current-rights indicator.
- App icons on window cards and list rows, with a short app name beside the icon (5, 6, or 7 characters; Appearance → App name length). The name is hidden when the window title already starts with it.
- A large app icon on the lower-left corner of each live thumbnail in Grid and Compact views; click it to switch to that window.
- Search box hint text ("Search by window title or app name").
- Alt+Tab diagnostics log at `%AppData%\Switchboard\logs\alttab-diagnostics.log` recording when the Windows switcher opened instead of Switchboard and which window was in front.

### Changed

- Overlay toolbar tooltips and footer key hints are now localized instead of always showing English.
- Switchboard now runs as a single instance per session.
- Simpler overlay toolbar: theme buttons moved to Settings only, and sorting is a single dropdown.
- Larger overlay toolbar (about 1.3x): taller header, search box, view buttons, sort dropdown, and icon buttons.
- Selected cards get a thicker highlight; the close button is larger (30 px) and appears only on the hovered or selected card.
- Alt+Tab while the overlay is open but covered or unfocused brings it to the front instead of closing it.
- The Alt+Tab hook responds faster under load (higher hook-thread priority, no power throttling, low-latency GC).

### Fixed

- When the Windows switcher opens instead of Switchboard, Switchboard now reinstalls its keyboard hook right away so later Alt+Tab presses come back to it (Windows can drop a hook without notice).
- After an Alt+Tab into an administrator window, the next Alt+Tab could be silently ignored.
- Selected view, theme, sort, and always-on-top buttons were nearly unreadable (white text on a pale accent).
- The window count in the overlay header is now localized.

### Verification

- Release build: 0 warnings, 0 errors
- Tests: 110/110 passed
- Verified the self-contained Windows 11 x64 single-file package

## [0.3.1] - 2026-08-18

A patch release that stabilizes the settings-window sidebar layout.

### Fixed

- Corrected the DockPanel child order that caused the “Changes saved” card to consume all remaining vertical space.
- Kept the Position, Appearance, and Default behavior tabs at the top and fixed the status card at its intended size at the bottom.

### Verification

- Release build: 0 warnings, 0 errors
- Tests: 57/57 passed
- Verified the self-contained Windows 11 x64 single-file package

## [0.3.0] - 2026-08-17

A feature release that overhauls the settings flow and persistently restores the overlay position chosen by the user.

### Added

- A separately owned modal settings window with Position and movement, Appearance and size, and Default behavior sidebar tabs
- Persistent position restoration: when an empty background drag ends, the current monitor, nine-position anchor, and margins are saved in JSON settings
- A nine-position visual remote and a Return to saved position button for one-time window movement
- 60% and 70% full-overlay scale options, plus a compatibility migration for existing scale values

### Changed

- Changed position calculation to retain the nearest saved screen edge or corner after changing size, view mode, or scale.
- Safely corrects the overlay into the current work area when its saved monitor is unavailable.

### Fixed

- Fixed clipping caused by applying the overlay scale twice to the fixed-size settings modal.

### Verification

- Release build: 0 warnings, 0 errors
- Tests: 57/57 passed
- Verified the self-contained Windows 11 x64 single-file package

## [0.2.1] - 2026-08-10

An icon patch release that improves Switchboard’s application identity.

### Changed

- Added a new Switchboard icon that combines four windows with bidirectional switching arrows.
- Standardized the tray, taskbar window, and executable icons on a multi-resolution Windows .ico asset with transparent pixels outside the charcoal tile.

### Verification

- Release build: 0 warnings, 0 errors
- Tests: 41/41 passed

## [0.2.0] - 2026-08-10

A feature release that adds a compact overlay and safe full-UI scaling, and fixes empty columns and preview overflow with many windows.

### Added

- A compact-overlay toggle, top-left, top-right, center, bottom-left, and bottom-right position choices, and JSON settings persistence
- A dedicated Switchboard icon identifiable in the system tray and taskbar

### Changed

- Unified full UI scale to 80%, 100%, 125%, 150%, and 200%, with 100% retained as the default.
- Automatically normalizes legacy 50%, 70%, 90%, and 120% scale settings to the nearest safe new value.
- Lets compact mode target 65% of the work area while expanding enough to show one complete card row; rows and columns are calculated from the window count and actual display area.
- Standardized the compact-position UI with the split-button style used for other fixed options.

### Fixed

- Matched the calculated and rendered column counts to fix unused right-side space and fewer-than-expected displayed columns.
- Fixed window-size and card-relayout mismatches when changing full scale and thumbnail scale together.
- Hid DWM previews outside the scroll area to prevent the window list from rendering outside the overlay.
- Fixed the settings panel and compact overlay exceeding the work area or being clipped at high scale.

### Known limitations

- When all windows cannot fit in the work area at once, Switchboard retains the calculated column count and uses vertical scrolling.
- A target application may not close immediately if it shows a save confirmation or rejects the close request.
- Administrator-elevated applications and the secure desktop are subject to Windows focus and messaging restrictions.

## [0.1.1] - 2026-07-20

A patch release that lets users close a target window directly from the window switcher.

### Added

- Added a normal-close button to every window card in grid, compact, and list views.
- Separated the Win32 WM_CLOSE request across the Core, Native, and App boundaries and added command regression tests.

### Fixed

- Fixed a close button that could be hidden behind the DWM preview; it now stays in the always-visible caption area.

### Known limitations

- A target application may not close immediately if it shows a save confirmation or rejects the close request.
- Close requests to administrator-elevated applications can be blocked by Windows messaging permissions.

## [0.1.0] - 2026-07-13

The first Windows 11 x64 Portable preview.

### Added

- Toggle Switchboard with one Alt+Tab press.
- Grid, compact, and list views with window-sorting options.
- Saved opacity, full UI scale, thumbnail size, and secondary-hotkey settings.
- System-tray presence and an always-on-top option.

### Improved

- Adjusted DWM thumbnails to show the full source window without clipping.
- Added responsive rows, columns, and scrolling for more than 25 windows.
- Optimized polling so unchanged window lists do not recreate the WPF/DWM UI.
- Brought the overlay in front of normal windows when it is opened while inactive.

### Known limitations

- Administrator-elevated windows and the secure desktop may not activate because of Windows focus restrictions.
- SmartScreen may appear because the distribution is not digitally code-signed.

[0.3.1]: https://github.com/ai-blink/alt-tab/releases/tag/v0.3.1
[0.3.0]: https://github.com/ai-blink/alt-tab/releases/tag/v0.3.0
[0.2.1]: https://github.com/ai-blink/alt-tab/releases/tag/v0.2.1
[0.2.0]: https://github.com/ai-blink/alt-tab/releases/tag/v0.2.0
[0.1.1]: https://github.com/ai-blink/alt-tab/releases/tag/v0.1.1
[0.1.0]: https://github.com/ai-blink/alt-tab/releases/tag/v0.1.0
