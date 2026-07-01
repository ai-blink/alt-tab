# Switchboard Init Plan

## 1. Goal

Create the first buildable Switchboard app skeleton: `.NET 10 + WPF` solution, clean App/Core/Native/Test boundaries, preserved Stitch references, and a mock window switcher shell.

Completion criteria:
- `dotnet build Switchboard.slnx --nologo` passes.
- `dotnet test Switchboard.slnx --nologo` passes.
- Stitch reference export is stored under `references/stitch/`.
- `CLAUDE.md` and live docs describe current state and next step.
- Visual acceptance requires a follow-up: the app launched as a process, but the user reported the UI is not yet visible/convincing enough.

## 2. Scope

In scope:
- Solution and project init.
- Mock-data WPF shell.
- Core search/sort/filter logic.
- Native placeholder boundary.
- Minimal live docs.

Out of scope:
- Full Alt+Tab interception.
- Virtual desktop management.
- Automatic window placement/resizing.
- Real DWM thumbnail rendering.

## 3. Approach

1. Initialize solution and project structure.
2. Copy Stitch design references.
3. Implement Core models/query logic and mock catalog.
4. Implement first WPF shell from thumbnail-grid direction.
5. Add smoke tests and build/test verification.
6. Initialize project docs and roadmap.

## 4. Verification

- Build solution.
- Run unit tests.
- Confirm reference files exist.
- Confirm docs point to next implementation slice.

## 5. Risks

- Native foreground switching has OS restrictions.
- DWM thumbnail rendering needs a focused prototype.
- Compact/List modes currently share the initial grid shell and need dedicated templates.
- The first WPF shell has build evidence, but needs a manual visual smoke and stronger launch visibility.
