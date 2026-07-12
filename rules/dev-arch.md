# Dev Architecture

## Layers

- `Switchboard.App`: WPF composition, resources, ViewModels, overlay lifecycle, and dispatcher coordination.
- `Switchboard.Core`: pure window models, query/selection services, settings enums, and responsive layout calculation.
- `Switchboard.Native`: Win32/DWM providers, hotkey registration, dedicated low-level keyboard hook thread, and foreground/z-order presentation.
- `Switchboard.Tests`: Core behavior plus pure App/Native boundary helpers; live HWND behavior remains runtime-smoke territory.

## Dependency Rules

- App may reference Core and Native; Native may reference Core models for provider output.
- Core must not reference App or Native.
- WPF must not own raw P/Invoke declarations; native handles and foreground policy stay in `Switchboard.Native`.
- Persistent `Topmost` state comes from the WPF setting; native presentation may change z-order only transiently and must leave the configured state intact.
- Low-level hook callbacks stay off the WPF UI thread and dispatch only bounded overlay actions.

## Verification

- Run `dotnet build Switchboard.slnx --nologo`.
- Run `dotnet test Switchboard.slnx --nologo`.
- For input/z-order changes, run repeated real-HWND smoke checks for visibility, foreground PID, `WS_EX_TOPMOST`, and previous-window restoration.
- For UI slices, visually check desktop behavior and the 900px minimum-width target.
