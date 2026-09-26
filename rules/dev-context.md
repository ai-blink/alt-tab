# Dev Context

- Date: 2026-09-27
- Current slice: Alt+Tab reliability (elevated sign-in task, single instance, hook responsiveness, diagnostics) plus UI review fixes and the chosen overlay UX (icons + short app names, toolbar 2A, cards 3A). Previous: four-language UI localization (UI check pending).
- Decision: UI strings live in per-language `ResourceDictionary` files (`src/Switchboard.App/Localization/Strings.{ko,en,zh-Hans,ja}.xaml`) read through `DynamicResource`; `AppLocalizer` owns the single merged string dictionary and swaps it at runtime, so language changes apply without restart.
- Decision: `AppLanguage.Auto` (default) follows the Windows display language; every Chinese variant (including Traditional) uses Simplified Chinese; unsupported languages fall back to English. `AppLocalizer` also sets each window's `xml:lang` so Han characters render with Chinese or Japanese glyphs.
- Decision: Alt+Tab over elevated windows is solved by an optional Task Scheduler sign-in task (`HighestAvailable`), not uiAccess, to keep Portable distribution; Switchboard is single-instance per session.
- Decision: the overlay persists user drag position by nearest 3×3 screen anchor plus offset. Position controls become a one-shot 9-direction picture remote plus saved-position return; only drag completion changes the permanent position.
- Decision: the settings `Popup` becomes a separate owner-modal window with left sidebar tabs: Position & Move, Appearance & Size, and Behavior.
- Decision: whole-overlay scale supports `60/70/80/100/125/150/200%`; Compact keeps automatic card layout. Overlay scale does not transform the fixed-size settings modal.
- Last verified: 2026-09-27 Debug build 0 warnings/errors, 94 tests pass; elevated vs standard Alt+Tab probe and task-verify register/delete passed.
- Immediate next step: user checks the new overlay UX (Compact/List, sort dropdown, app name length); user enables sign-in auto-start and confirms an elevated run after sign-out/in; then the pending language and `v0.3.1` UI checks in `dev-progress.md`.
- Watch item: WPF logical coordinates versus Win32 physical work areas on mixed-DPI monitors; use a safe visible-work-area fallback.
