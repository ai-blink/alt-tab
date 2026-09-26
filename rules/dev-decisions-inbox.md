# Dev Decisions Inbox

| Date | Decision | Why |
|---|---|---|
| 2026-09-27 | App UI i18n uses per-language WPF `ResourceDictionary` files + `DynamicResource` (not `.resx`, not a custom string class). `AppLanguage.Auto` follows the Windows display language, falling back to English. | User choice: WPF-native, runtime switching without restart, no extra wrapper layer; key parity is enforced by `LocalizationTests` instead of the compiler. |

승인·통합된 결정의 정본은 [`dev-decisions.md`](dev-decisions.md)입니다. 새 결정 후보만 이 파일에 추가하고, 통합한 항목은 정본으로 옮긴 뒤 제거합니다.
