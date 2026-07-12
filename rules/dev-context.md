# Dev Context

- Date: 2026-07-13
- Current slice: Alt+Tab overlay semantics and runtime stability are complete.
- Completed: dedicated low-level hook thread, one-gesture visible/hidden toggle, previous-window restoration, full-source DWM thumbnail fitting, count-aware responsive layout beyond 25 windows, and stable polling without unchanged-list redraws.
- Always-on-top policy: `IsAlwaysOnTop` owns persistent topmost state; non-topmost activation uses an immediate `TOPMOST -> NOTOPMOST` pulse so the overlay reaches the front without leaving `WS_EX_TOPMOST` set.
- Verification: build passes with 0 warnings/errors; 12 tests pass; 10 runtime gestures produced `VHVHVHVHVH` with 0 visibility, foreground, topmost, or restoration failures.
- Runtime state: the latest Debug build is resident in the tray after smoke verification.
- Next step: add user-facing feedback for reserved configurable hotkeys or replace catalog polling with Win32 event updates.
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
