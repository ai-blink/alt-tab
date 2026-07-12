# Switchboard July 2026 Progress Archive

Detailed implementation notes and visual artifacts remain under `notes/runs/`; git history preserves the original live-progress narration.

- Initialized the `.NET 10` WPF App/Core/Native/Tests solution and copied the Stitch reference pack.
- Reworked the first shell into a compact transparent thumbnail-first overlay with light/dark appearance modes.
- Connected Win32 top-level enumeration, DWM thumbnails, keyboard/double-click activation, and Grid/Compact/List templates.
- Added responsive count-sized bounds, thumbnail/app scale presets, sorting, opacity, always-on-top, and settings persistence.
- Added tray residency and configurable Win32 global hotkey registration.
- Removed native enumeration caps and aligned scored responsive grid calculations with rendered slots.
- Moved Alt+Tab capture to a dedicated low-level-hook thread and made it a visible/hidden overlay toggle.
- Restored previous foreground windows after hiding and separated transient presentation from persistent topmost policy.
- Changed DWM previews to full-source contain fitting and prevented unchanged polling from rebuilding visual trees.
- Current verification and resume state live in `rules/dev-progress.md` and `rules/dev-context.md`.
