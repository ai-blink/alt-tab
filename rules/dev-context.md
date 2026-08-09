# Dev Context

- Date: 2026-08-09
- Current slice: approved compact-overlay Option 1 is ready for WPF implementation.
- Decision: compact mode keeps automatic card layout, adds five independent window-size levels and five UI-size levels, and retains selectable display position. Manual maximum rows/columns are not part of the approved option.
- Current baseline: the committed prototype persists compact mode and location, but its `PresentationScale` 70% and 65% work-area cap shrink the whole UI together. The next slice must replace that behavior with the approved separation.
- Completed: custom Switchboard icon now appears in the tray and taskbar window; the Release app was launched successfully.
- Verification: Release build passes with 0 warnings/errors; 19 tests pass; HTML mock JavaScript syntax and control counts pass.
- Runtime state: the current Release build is resident in the tray after launch verification.
- Next step: implement approved Option 1, then add focused layout/settings tests and a runtime smoke check.
- Release target: next Windows 11 x64 Portable build (current version `0.1.1`).
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
