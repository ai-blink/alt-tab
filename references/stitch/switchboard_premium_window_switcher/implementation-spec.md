# Stitch Implementation Spec

Source folder: `references/stitch/switchboard_premium_window_switcher`

Role: WPF visual-translation notes for the imported Stitch reference. Current product behavior is defined by `README.md`, `rules/`, and the application code; the imported [`DESIGN.md`](switchboard_desktop_utility/DESIGN.md) remains unchanged.

## Candidate Decision

- Primary: `switchboard_thumbnail_grid_view`
- Borrow from: `switchboard_command_palette_view`, `switchboard_dense_list_view`, `switchboard_monitor_grouped_view`
- Later: `switchboard_mini_dock_view`
- Avoid: `switchboard_timeline_history_view`

## WPF Translation

- Use a borderless `Window` with a compact top toolbar and a content-sized switcher surface.
- Use one `ListBox` with an exact-column `UniformGrid`; bind the calculated column count so layout and rendering agree.
- Keep distinct `DataTemplate` instances for Grid, Compact, and List views.
- Render Win32/DWM thumbnails through a model-bound WPF host control and keep raw interop in `Switchboard.Native`.
- Keep infrequent settings in a fixed-size owner-modal sidebar window so overlay scaling does not clip its contents.

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
