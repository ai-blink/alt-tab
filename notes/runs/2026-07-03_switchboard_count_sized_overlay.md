# Switchboard Count-Sized Overlay Run

Date: 2026-07-03

## Finish Line

Capture the post-`0a15adf` baseline where Switchboard sizes the overlay and grid columns from the current filtered window count, keeping all windows visible when desktop bounds allow instead of depending on an in-overlay scrollbar.

## Acceptance Checks

- The overlay starts with manual centered bounds rather than default `CenterScreen` startup sizing.
- Grid and compact layouts use a calculated column count from the current window count and available screen width.
- The shell grows toward the number of visible windows while respecting primary-screen bounds.
- The visible scrollbar is hidden in the main switcher surface.
- Existing Grid, Compact, and List templates remain available.

## Scope Limit

- No settings UI in this baseline note.
- No settings persistence.
- No foreground activation fallback UX.
- No Alt+Tab interception or OS replacement behavior.

## Changes Recorded From `0a15adf`

- Added `ApplyContentSizedBounds` and layout calculation in `MainWindow.xaml.cs`.
- Added `GridColumnCount` to `MainWindowViewModel` and synchronized it from shell layout.
- Switched the overlay startup location to manual centering after size calculation.
- Hid the main switcher vertical scrollbar.
- Captured `notes/runs/2026-07-02_switchboard_count_sized_desktop_smoke.png`.

## Verification

- Runtime smoke artifact: `notes/runs/2026-07-02_switchboard_count_sized_desktop_smoke.png`.
- Visual baseline confirmed the count-sized desktop layout after `0a15adf`.

## Review Budget

Documentation catch-up only; no implementation loop used in this note.

## Stop Rule

Baseline is recorded. Next implementation slice is the compact settings button, settings popover, and thumbnail scale control.
