# Dev Context

- Date: 2026-07-02
- Current slice: project init and first WPF shell.
- Completed: solution, App/Core/Native/Tests projects, Stitch reference copy, mock grid UI, Core query tests.
- Current feedback: app launches/builds, but user reported the UI is not yet visibly present or convincing enough at runtime.
- Next step: make the WPF shell unmistakably visible on launch and run a manual visual smoke before Native enumeration.
- Watch item: `SetForegroundWindow` may fail under Windows foreground-lock rules; design fallback UX before relying on it.
