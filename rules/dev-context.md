# Dev Context

- Date: 2026-08-09
- Current slice: approved compact-overlay Option 1 is implemented and verified in Release.
- Decision: compact mode keeps selectable position and independent five-step window/UI sizes, and now lets users set maximum card columns (4~7) and rows (4~6). Actual layout remains automatic inside those limits.
- Current behavior: compact window size controls the work-area cap (45%/55%/65%/75%/85%), while UI size controls only the overlay render scale (85%/92.5%/100%/110%/120%). When the selected grid needs more room (for example 5×5), its required bounds take precedence up to the physical work area. Defaults are `보통` window size and `크게` UI size.
- Completed: custom Switchboard icon now appears in the tray and taskbar window; the Release app was launched successfully.
- Verification: Release build passes with 0 warnings/errors; 23 tests pass; focused compact layout/settings tests pass; HTML mock JavaScript syntax and control counts pass.
- Runtime state: the current Release build is resident in the tray after launch verification.
- Next step: restart the resident app only when the user is ready, then visually smoke-test compact sizes and the 4~7 columns / 4~6 rows limits.
- Release target: next Windows 11 x64 Portable build (current version `0.1.1`).
- Watch item: elevated/security-desktop windows remain outside the guaranteed foreground-activation boundary.
