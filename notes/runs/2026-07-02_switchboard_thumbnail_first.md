# Run Report - Thumbnail-First Cards

## Finish Line

Reduce title dominance and make real DWM thumbnails the primary content in each compact switcher card.

## Changes

- Changed each card from a split thumbnail/title layout to a thumbnail-first tile.
- Moved app/title text into a single-line bottom caption.
- Separated DWM thumbnail rendering from the caption area so DWM does not paint over text.
- Tuned card width and margins to keep the 3x2 layout.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed with 0 warnings and 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 2 tests.
- Manual visual smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe` and captured `notes/runs/2026-07-02_switchboard_thumbnail_first_smoke.png`.
