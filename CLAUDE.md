# CLAUDE.md - Switchboard

Switchboard is a Windows 11 window switcher utility. It aims to make window switching clearer than the default Alt+Tab by providing stable, compact, user-configurable views with readable thumbnails and titles.

This file is the canonical project instruction entrypoint. Do not create a competing authoritative `AGENTS.md`; if compatibility is ever needed, make it a thin pointer to this file.

## Product Target

- Build a commercial-quality Windows desktop overlay for general Windows users.
- First version: `.NET 10 LTS + WPF`, mock-data UI, clean Core/Native/App boundaries, and a path toward Win32/DWM integration.
- Primary view direction comes from `references/stitch/switchboard_premium_window_switcher`.

## Non-Goals For V1

- Do not fully intercept or replace the OS Alt+Tab.
- Do not build advanced virtual desktop management.
- Do not build automatic window placement or resizing.
- Do not ship mini dock, timeline, or workspace management in the first slice.

## Completion Discipline

For non-trivial work:
- Define `Finish Line`, `Acceptance Checks`, `Scope Limit`, `Review Budget`, and `Stop Rule` before implementation.
- Fix only `BLOCKER` issues that prevent the finish line or checks from passing.
- Record `FOLLOW_UP` and `IGNORE_FOR_V1` in `rules/dev-roadmap.md` or `rules/dev-progress.md`.
- Use at most two fix/review loops unless the user explicitly approves more.
- Stop after acceptance checks pass and move adjacent improvements to follow-up.

## Commands

- Build: `dotnet build Switchboard.slnx --nologo`
- Test: `dotnet test Switchboard.slnx --nologo`
- Run app: `dotnet run --project src/Switchboard.App/Switchboard.App.csproj`

## Project Map

- `src/Switchboard.App`: WPF shell, ViewModels, resources, overlay UI.
- `src/Switchboard.Core`: window models, filtering, sorting, settings-domain logic.
- `src/Switchboard.Native`: Win32, DWM, hotkey, and foreground activation boundaries.
- `tests/Switchboard.Tests`: unit tests for Core behavior first.
- `rules/`: live development docs and roadmap.
- `references/stitch/`: Stitch design exports and WPF handoff notes.
