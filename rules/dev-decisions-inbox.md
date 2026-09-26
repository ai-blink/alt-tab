# Dev Decisions Inbox

| Date | Decision | Why |
|---|---|---|
| 2026-09-27 | App UI i18n uses per-language WPF `ResourceDictionary` files + `DynamicResource` (not `.resx`, not a custom string class). `AppLanguage.Auto` follows the Windows display language (any Chinese variant → Simplified Chinese), falling back to English. | User choice: WPF-native, runtime switching without restart, no extra wrapper layer; key parity is enforced by `LocalizationTests` instead of the compiler. |
| 2026-09-27 | Alt+Tab over elevated windows uses an optional Task Scheduler sign-in task with `HighestAvailable` (not uiAccess); Switchboard is single-instance per session. | User choice: keeps Portable distribution; uiAccess needs a trusted code-signing certificate plus a Program Files install. Two instances would double-toggle. |
| 2026-09-27 | Overlay UX mocks: section 1 = one-line icon + app name (5/6/7 chars, hidden when the title starts with it) plus 1-C translucent large icon over the thumbnail corner; section 2 = A; section 3 = A with a 24px close button. 1-C uses a separate owned click-through layer window. | User choices. DWM thumbnails cover same-window WPF content, so a real overlay needs its own window; shrinking the thumbnail was rejected. |

승인·통합된 결정의 정본은 [`dev-decisions.md`](dev-decisions.md)입니다. 새 결정 후보만 이 파일에 추가하고, 통합한 항목은 정본으로 옮긴 뒤 제거합니다.
