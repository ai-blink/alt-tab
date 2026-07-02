# Switchboard Keyboard Activation Run

Date: 2026-07-02

## Finish Line

The compact overlay supports keyboard card selection and can request foreground activation for the selected real top-level window without enlarging the UI.

## Scope

- Tab and Shift+Tab cycle selected cards.
- Arrow keys move selection through the 3-column card grid.
- Enter calls the Native foreground activation boundary for the selected window.
- Esc keeps the existing close behavior.
- No layout expansion or mode redesign.

## Changes

- Added `WindowSelection` in Core for tested circular selection behavior.
- Added `IWindowActivator` and implemented it in `Win32NativeWindowProvider` with `ShowWindow(SW_RESTORE)`, `BringWindowToTop`, and `SetForegroundWindow`.
- Wired WPF `PreviewKeyDown` to selection movement and Enter activation.
- Kept the selected card visible with the existing compact ListBox surface.

## Verification

- `dotnet build Switchboard.slnx --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test Switchboard.slnx --nologo`: passed, 4 tests.
- Runtime smoke: launched `src/Switchboard.App/bin/Debug/net10.0-windows/Switchboard.App.exe` and captured `notes/runs/2026-07-02_switchboard_keyboard_activation_smoke.png`.

## Notes

- Windows may still reject foreground activation under foreground-lock rules. Current behavior keeps the overlay open if activation is not confirmed; a visible fallback should be designed later.
