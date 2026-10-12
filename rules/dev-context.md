# Dev Context

- Date: 2026-10-12
- Current slice: v0.4.2 patch — Normal-priority Alt+Tab dispatch, one pending gesture, and cancellation of stale toggles when opening from the tray. Native activation and v0.4.1 menu/capture cleanup are retained. User confirmed normal immediate response; long-term recurrence remains under observation.
- Decision: UI strings live in per-language `ResourceDictionary` files (`src/Switchboard.App/Localization/Strings.{ko,en,zh-Hans,ja}.xaml`) read through `DynamicResource`; `AppLocalizer` owns the single merged string dictionary and swaps it at runtime, so language changes apply without restart.
- Decision: `AppLanguage.Auto` (default) follows the Windows display language; every Chinese variant (including Traditional) uses Simplified Chinese; unsupported languages fall back to English. `AppLocalizer` also sets each window's `xml:lang` so Han characters render with Chinese or Japanese glyphs.
- Decision: Alt+Tab over elevated windows is solved by an optional Task Scheduler sign-in task (`HighestAvailable`), not uiAccess, to keep Portable distribution; Switchboard is single-instance per session.
- Decision: the overlay persists user drag position by nearest 3×3 screen anchor plus offset. Position controls become a one-shot 9-direction picture remote plus saved-position return; only drag completion changes the permanent position.
- Decision: the settings `Popup` becomes a separate owner-modal window with left sidebar tabs: Position & Move, Appearance & Size, and Behavior.
- Decision: whole-overlay scale supports `60/70/80/100/125/150/200%`; Compact keeps automatic card layout. Overlay scale does not transform the fixed-size settings modal.
- Last verified: 2026-10-12 build 0 warnings/errors, 117 tests pass; user confirmed normal Alt+Tab/tray response in the deployed fix. Long-term global input-stall resolution remains unverified.
- Immediate next step: observe recurrence of intermittent Alt+Tab stalls and desktop-wide click blocking; capture Esc recovery and overlay-input logs before changing native activation. Remaining UI/sign-in checks are in dev-progress.md.
- Watch item: WPF logical coordinates versus Win32 physical work areas on mixed-DPI monitors; use a safe visible-work-area fallback.
