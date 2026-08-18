# Dev Context

- Date: 2026-08-18
- Current slice: `v0.3.1` Portable patch release for the settings sidebar status-card layout.
- Decision: the overlay persists user drag position by nearest 3×3 screen anchor plus offset. Position controls become a one-shot 9-direction picture remote plus saved-position return; only drag completion changes the permanent position.
- Decision: the settings `Popup` becomes a separate owner-modal window with left sidebar tabs: Position & Move, Appearance & Size, and Behavior.
- Decision: whole-overlay scale supports `60/70/80/100/125/150/200%`; Compact keeps automatic card layout.
- Decision: overlay scale does not transform the fixed-size settings modal; Windows DPI handles the modal so its contents remain within its frame.
- Plan: `notes/plans/2026-08-14-settings-window-overhaul.md`; UX design: `notes/brainstorm/20260814_settings-window-overhaul_design.md`.
- Last verified: 2026-08-18 `v0.3.1` Release build has 0 warnings/errors and all 57 tests pass. The self-contained `win-x64` ZIP contains only `Switchboard.App.exe` and `읽어주세요.txt`.
- Immediate next step: complete the user UI check for the settings sidebar status card, then verify 60/70/100/200%, Grid/Compact/List, drag persistence, 10 remote actions, and modal close/focus behavior.
- Watch item: WPF logical coordinates versus Win32 physical work areas on mixed-DPI monitors; use a safe visible-work-area fallback.
