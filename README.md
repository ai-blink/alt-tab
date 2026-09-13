# Switchboard

[English](README.md) | [한국어](README.ko.md) | [中文](README.zh-CN.md) | [日本語](README.ja.md)

Switchboard is a Windows 11 desktop overlay that makes switching between windows clearer and more dependable. It shows open windows with readable titles and uncropped previews, then adapts its layout to the number of windows and the available screen space.

**Current version:** [v0.3.1](https://github.com/ai-blink/alt-tab/releases/tag/v0.3.1) · **Distribution:** Windows 11 x64 Portable

> This is a portable preview: extract the archive and run it without an installer. Builds without code signing may trigger a Windows SmartScreen warning.

## Features

- Toggle Switchboard with one Alt+Tab press
- DWM previews that show each source window in full
- Responsive rows, columns, and scrolling for more than 25 windows
- Grid, compact, and list views
- Saved overlay position from dragging empty space, a one-time nine-position remote, and return to the saved position
- Sorting by most recently used, app, monitor, title, or favorites
- Transparent, dark, and light themes; opacity; UI scale (60/70/80/100/125/150/200%); and thumbnail-size settings
- Custom secondary hotkey (Ctrl+Alt+Space by default)
- Always-on-top option, system-tray presence, and a dedicated app icon that signals window switching
- A close button on every window card that requests a normal application close
- Refreshes only when the window list changes to minimize polling flicker

## Install and run

1. Extract Switchboard-v0.3.1-win-x64-Portable.zip into a new folder.
2. Run Switchboard.App.exe.
3. If SmartScreen appears, verify the source, then choose **More info → Run anyway**.
4. To exit, right-click the Switchboard icon in the taskbar notification area and choose **Exit**.

The self-contained distribution includes the .NET runtime, so no separate runtime installation is required.

## Controls

| Input | Action |
| --- | --- |
| Alt+Tab | Toggle the overlay |
| Ctrl+Alt+Space | Show the overlay (default secondary hotkey) |
| Tab or arrow keys | Move the window-card selection |
| Enter | Activate the selected window |
| Esc | Hide the overlay |
| Double-click | Activate that window |
| X button on a window card | Request that window to close normally |

The settings button at the top opens a separate modal window. Its left tabs organize position and movement, appearance and size, and default behavior. Dragging empty overlay space saves the new position automatically; the position remote only moves the overlay once and does not change the saved position. The UI-scale setting changes text, buttons, spacing, and window size together, and compact mode does not apply an additional scale reduction. The pin button toggles **Always on top**.

The close button sends the standard Windows close request; it does not force-quit an application. If the target application has unsaved work, its usual save-confirmation dialog may appear.

## Settings file

Settings are saved automatically at:

~~~
%APPDATA%\Switchboard\settings.json
~~~

To restore defaults, exit Switchboard, delete that file, and start Switchboard again.

## Known limitations

- Activating administrator-elevated windows or windows on the secure desktop can fail because of Windows focus restrictions.
- The current distribution is not digitally code-signed, so SmartScreen may appear.
- There is currently no setting to disable the Alt+Tab hook. Exit Switchboard to use the default Windows Alt+Tab behavior.
- Virtual-desktop management, automatic window placement, and timeline features are outside V1 scope.

## Development

- Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- WPF / Win32 / DWM

~~~powershell
git clone https://github.com/ai-blink/alt-tab.git
cd alt-tab
dotnet build Switchboard.slnx --nologo
dotnet test Switchboard.slnx --nologo
dotnet run --project src/Switchboard.App/Switchboard.App.csproj
~~~

## Release build

Create a Windows x64, self-contained, single-file executable with:

~~~powershell
dotnet publish src/Switchboard.App/Switchboard.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  --nologo `
  -o artifacts/release/win-x64 `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:PublishTrimmed=false `
  -p:DebugType=None `
  -p:DebugSymbols=false
~~~

Release ZIP files use the Switchboard-v{version}-win-x64-Portable.zip naming convention. See [CHANGELOG.md](CHANGELOG.md) for version-by-version changes.

## Project documentation

| Document | Purpose |
| --- | --- |
| [CLAUDE.md](CLAUDE.md) | The sole canonical entry point for project working rules and standard commands |
| [Current context](rules/dev-context.md) · [Progress](rules/dev-progress.md) · [Roadmap](rules/dev-roadmap.md) | Current slice, validation status, and next work |
| [Architecture](rules/dev-arch.md) · [UX principles](rules/dev-ux.md) · [Decisions](rules/dev-decisions.md) | Implementation boundaries and product decisions that remain in effect |
| [Plans](notes/plans/) · [Run records](notes/runs/) · [Stitch references](references/stitch/) | Historical plans and validation evidence; not the source of truth for current status |

## Project structure

- src/Switchboard.App: WPF shell, views, view models, and user settings
- src/Switchboard.Core: Window models, filtering, sorting, and layout calculations
- src/Switchboard.Native: Win32/DWM window enumeration, hotkeys, and foreground activation
- tests/Switchboard.Tests: Core plus input and refresh behavior tests

Please report issues in [GitHub Issues](https://github.com/ai-blink/alt-tab/issues) with reproduction steps and your Windows version.
