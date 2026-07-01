# Dev Architecture

## Layers

- `Switchboard.App`: WPF composition, resources, ViewModels, commands. No direct Win32 calls.
- `Switchboard.Core`: pure domain logic for window snapshots, view modes, sort modes, search/filter/sort.
- `Switchboard.Native`: P/Invoke, HWND, DWM thumbnail, global hotkey, foreground activation wrappers.
- `Switchboard.Tests`: Core tests first; Native integration tests only after wrappers stabilize.

## Dependency Rules

- App may reference Core and Native.
- Native may reference Core models for provider output.
- Core must not reference App or Native.
- UI uses mock catalog until native enumeration is ready.

## Verification

- Run `dotnet build Switchboard.slnx --nologo`.
- Run `dotnet test Switchboard.slnx --nologo`.
- For UI slices, run the WPF app and visually check desktop and 900px minimum width.
