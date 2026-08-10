# Dev Context

- Date: 2026-08-10
- Current slice: the approved Switchboard app icon is being packaged as the `v0.2.1` Windows 11 x64 Portable patch release.
- Decision: overall UI scale is the single `80/100/125/150/200%` control. Compact mode keeps automatic card layout and selectable position, but does not apply an additional UI scale or expose manual row/column limits.
- Current baseline: compact mode targets 65% of the work area, expands when needed to keep one complete card row visible, and never exceeds the work area. DWM previews outside the list viewport are hidden.
- Completed: the blue-violet four-pane Switchboard icon has transparent outer corners and is used by the tray, taskbar window, and executable.
- Verification: `v0.2.1` Release build passes with 0 warnings/errors; 41 tests pass; the Portable ZIP contains the self-contained executable and Korean guide.
- Runtime state: all Switchboard verification processes are stopped after the package smoke.
- Next step: publish the approved `v0.2.1` Portable ZIP and visually inspect the icon at taskbar/tray sizes; retain scale checks as follow-up runtime coverage.
- Release target: `v0.2.1` Windows 11 x64 Portable.
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
