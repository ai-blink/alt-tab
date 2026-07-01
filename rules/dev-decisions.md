# Dev Decisions

## Vision

- D-001: Switchboard targets a commercial-quality Windows utility feel, not a landing page or demo mock.
- D-002: V1 optimizes fast finding and readable thumbnails/titles before advanced workspace features.

## Layer Boundaries

- D-003: Keep WPF UI and Win32/DWM interop separated through `Switchboard.Native`.
- D-004: Keep search/sort/filter logic in `Switchboard.Core` so it remains unit-testable.

## UX

- D-005: Primary design reference is Stitch `switchboard_thumbnail_grid_view`.
- D-006: Borrow search/keyboard hints from command-palette, long-title handling from dense-list, monitor headers from monitor-grouped.

## Workflow

- D-007: `CLAUDE.md` is the single canonical project instruction entrypoint.
