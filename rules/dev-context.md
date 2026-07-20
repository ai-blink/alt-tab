# Dev Context

- Date: 2026-07-20
- Current slice: `v0.1.1` window-close controls are complete and ready for portable release.
- Completed: each Grid/Compact/List card can send a standard `WM_CLOSE` request through the Core/Native/App boundary; controls live outside the DWM preview destination so they remain visible.
- Always-on-top policy: `IsAlwaysOnTop` owns persistent topmost state; non-topmost activation uses an immediate `TOPMOST -> NOTOPMOST` pulse so the overlay reaches the front without leaving `WS_EX_TOPMOST` set.
- Verification: Release build passes with 0 warnings/errors; 13 tests pass; prior 10-gesture runtime stability evidence remains valid.
- Runtime state: the latest Debug build is resident in the tray after smoke verification.
- Next step: add user-facing feedback for reserved configurable hotkeys or replace catalog polling with Win32 event updates.
- Release target: `v0.1.1`, Windows 11 x64 Portable.
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
