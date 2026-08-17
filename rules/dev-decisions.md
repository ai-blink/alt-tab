# Dev Decisions

## Vision

- D-001: Switchboard targets a commercial-quality Windows utility feel, not a landing page or demo mock.
- D-002: V1 optimizes fast finding and readable thumbnails/titles before advanced workspace features.

## Layer Boundaries

- D-003: Keep WPF UI and Win32/DWM interop separated through `Switchboard.Native`.
- D-004: Keep search, sort, selection, and responsive layout calculations testable outside WPF rendering.

## UX

- D-005: Primary design reference is Stitch `switchboard_thumbnail_grid_view`.
- D-006: Borrow search/keyboard hints from command-palette, long-title handling from dense-list, and monitor headers from monitor-grouped.
- D-008: Alt+Tab toggles Switchboard visibility; plain Tab/arrows navigate cards and Enter activates the selected window.
- D-009: `IsAlwaysOnTop` is the sole persistent topmost policy; foreground presentation may pulse topmost only when it immediately restores normal z-order.
- D-010: Overall UI scale is a single 60/70/80/100/125/150/200% setting. Compact mode controls available bounds only; it adds no separate scale and exposes no manual row/column limits.
- D-011: Persist overlay position only after direct drag, using the nearest 3×3 work-area anchor plus offset. The picture remote performs one-shot movement and never replaces the saved position.
- D-012: Replace the anchored settings popup with an owner-modal settings window whose left sidebar switches Position & Move, Appearance & Size, and Behavior content.
- D-013: Apply the configurable scale to the overlay only; keep the fixed-size settings modal untransformed so its content cannot outgrow its window frame.

## Workflow

- D-007: `CLAUDE.md` is the single canonical project instruction entrypoint.
