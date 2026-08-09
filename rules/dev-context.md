# Dev Context

- Date: 2026-08-10
- Current slice: unified overlay scaling and compact-layout recovery fixes are implemented and Release-verified.
- Decision: overall UI scale is the single `80/100/125/150/200%` control. Compact mode keeps automatic card layout and selectable position, but does not apply an additional UI scale or expose manual row/column limits.
- Current baseline: compact mode targets 65% of the work area, expands when needed to keep one complete card row visible, and never exceeds the work area. DWM previews outside the list viewport are hidden.
- Completed: custom Switchboard icon now appears in the tray and taskbar window; the Release app was launched successfully.
- Verification: Release build passes with 0 warnings/errors; 40 tests pass; focused scale/layout/settings tests pass.
- Runtime state: the new Release build has not been launched, so visual checks for the five scale extremes remain a user UI smoke step.
- Next step: launch the Release app on request and visually check 80% compact, 125% with 1.2x thumbnails, and 200% compact.
- Release target: next Windows 11 x64 Portable build (current version `0.1.1`).
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
