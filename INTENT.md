# Switchboard Intent

Switchboard helps general Windows users find and switch to the right window faster than the default Alt+Tab when many windows are open.

The product should feel like a polished desktop utility: compact, predictable, keyboard-friendly, readable, and calm. Users should be able to choose Grid, Compact, or List views and sort windows by Recent, App, Monitor, Title, or Favorites.

V1 focuses on a trustworthy overlay experience and internal architecture. Native Win32/DWM behavior should be isolated behind `Switchboard.Native` so WPF UI work stays clean and testable.
