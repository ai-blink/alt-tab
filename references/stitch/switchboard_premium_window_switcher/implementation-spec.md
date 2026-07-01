# Stitch Implementation Spec

Source folder: `references/stitch/switchboard_premium_window_switcher`

## Candidate Decision

- Primary: `switchboard_thumbnail_grid_view`
- Borrow from: `switchboard_command_palette_view`, `switchboard_dense_list_view`, `switchboard_monitor_grouped_view`
- Later: `switchboard_mini_dock_view`
- Avoid: `switchboard_timeline_history_view`

## WPF Translation

- Use `Window` + top toolbar + optional left rail + content area.
- Use `ListBox` or `ItemsControl` with `WrapPanel` for the Grid view.
- Use `DataTemplate` per view mode once Compact/List become distinct.
- Keep Win32/DWM thumbnails behind model-bound host controls later; initial shell may use mock thumbnail surfaces.

## Tokens

- Primary: `#0067C0`
- Surface: `#FCF9F8`
- Card: `#FFFFFF`
- Panel: `#F6F3F2`
- Border: `#C1C6D4`
- Text: `#1C1B1B`
- Muted text: `#414752`
- Base spacing: 4px
- Container padding: 24px
- Card radius: 8px max

## Acceptance Checks

- Thumbnails and titles are visible at desktop size and 900px minimum width.
- Selected state is visible without relying on color alone.
- Grid, Compact, and List mode controls are present.
- Search and sort controls are visible and keyboard reachable.
