# Switchboard August 2026 Progress Archive

Release details remain in `CHANGELOG.md`; implementation experiments and runtime artifacts remain under `notes/runs/` and `notes/brainstorm/`.

- Replaced the compact-only presentation multiplier with one overlay scale model and exact-column `UniformGrid` rendering.
- Kept Compact layout automatic, constrained DWM previews to the visible viewport, and normalized legacy JSON scale values.
- Shipped the `v0.2.0` Portable release with a 0-warning Release build and 41 passing tests.
- Unified tray, taskbar, and executable branding in `v0.2.1`; package SHA-256: `25EA0A84275CC320934001669DD087305ABE00454E8D4D5C896520C19333DCBB`.
- Replaced the anchored settings popup with a fixed-size owner-modal sidebar window in `v0.3.0`.
- Added drag-only 3×3 anchor position persistence, one-shot 9-direction movement plus saved-position return, 60/70% scale presets, and safe work-area fallback.
- Verified `v0.3.0` with 57 tests; package SHA-256: `A3A84823263E04267120D5EA20FFA3F505F503DCC1C2934C0DA08C20F9640A73`.
- Fixed the expanding settings-sidebar status card and published `v0.3.1`; package SHA-256: `B1144AE4C5F6322C4C35F0CF2BE9B1FC7E72AD078744B81FD3EF64B8D35F10C6`.
- Current verification and resume state live in `rules/dev-progress.md` and `rules/dev-context.md`.
