# Dev Context

- Date: 2026-08-10
- Current slice: the completed compact-overlay and unified-scaling work is packaged as the local `v0.2.0` Windows 11 x64 Portable release.
- Decision: overall UI scale is the single `80/100/125/150/200%` control. Compact mode keeps automatic card layout and selectable position, but does not apply an additional UI scale or expose manual row/column limits.
- Current baseline: compact mode targets 65% of the work area, expands when needed to keep one complete card row visible, and never exceeds the work area. DWM previews outside the list viewport are hidden.
- Completed: custom Switchboard icon now appears in the tray and taskbar window; the Release app was launched successfully.
- Verification: `v0.2.0` Release build passes with 0 warnings/errors; 41 tests pass; the packaged self-contained executable remained responsive during a process smoke test.
- Runtime state: all Switchboard verification processes are stopped after the package smoke.
- Next step: retain visual checks for 80% compact, 125% with 1.2x thumbnails, and 200% compact as follow-up runtime coverage; remote publication requires an explicit publish request.
- Release target: `v0.2.0` Windows 11 x64 Portable.
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
