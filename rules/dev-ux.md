# Dev UX

- Use Stitch as design reference, not production HTML.
- Primary view: thumbnail grid with stable cards, readable titles, monitor context, and an obvious selected state.
- Supporting views: compact mode for denser scanning and list mode for long titles.
- Selected state must not be color-only: combine border, tint, and focus affordance.
- Alt+Tab toggles Switchboard visible/hidden; plain Tab and arrows navigate cards; Enter activates the selection.
- Activation must bring a non-topmost overlay to the head of normal z-order without leaving it always on top.
- Keep cards at 8px radius or less and use 1px borders with restrained shadows.
- Avoid decorative blobs, marketing hero layouts, and purple/blue gradient-heavy themes.
- Text must fit at 900px minimum width; long titles should trim predictably.
