# Switchboard Thumbnail Fit Run

Date: 2026-07-05

## Finish Line

Make DWM thumbnails fit the card preview host consistently across Grid, Compact, and List after whole-overlay app scaling.

## Acceptance Checks

- DWM thumbnail destination bounds include the rendered transform, not only the logical top-left point.
- Thumbnail previews fill their host rectangles consistently instead of varying the destination size per source aspect ratio.
- Grid, Compact, and List modes still render with live DWM previews.
- Build, tests, and runtime smoke pass.

## Scope Limit

- No new thumbnail settings.
- No overlay layout score changes.
- No persistence changes.

## Changes

- Changed `DwmThumbnailPreview` to transform the full rendered preview bounds before converting to DWM device-pixel rectangles.
- Changed DWM destination rectangles to use the full preview host area.
- Replaced soft letterbox sizing with source cropping for cover-style thumbnail fit.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe`, selected Grid/Compact/List via UI Automation, and captured:
  - `notes/runs/2026-07-05_switchboard_thumbnail_fit_grid_smoke.png`
  - `notes/runs/2026-07-05_switchboard_thumbnail_fit_compact_smoke.png`
  - `notes/runs/2026-07-05_switchboard_thumbnail_fit_list_smoke.png`

## Review Budget

Used 1 implementation/review loop.

## Stop Rule

Acceptance checks passed. Future visual tuning can adjust crop policy if users prefer letterboxed previews.
