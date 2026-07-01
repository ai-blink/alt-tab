---
name: Switchboard Desktop Utility
colors:
  surface: '#fcf9f8'
  surface-dim: '#dcd9d9'
  surface-bright: '#fcf9f8'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f6f3f2'
  surface-container: '#f0eded'
  surface-container-high: '#eae7e7'
  surface-container-highest: '#e5e2e1'
  on-surface: '#1c1b1b'
  on-surface-variant: '#414752'
  inverse-surface: '#313030'
  inverse-on-surface: '#f3f0ef'
  outline: '#717783'
  outline-variant: '#c1c6d4'
  surface-tint: '#005eb1'
  primary: '#004f96'
  on-primary: '#ffffff'
  primary-container: '#0067c0'
  on-primary-container: '#dbe7ff'
  inverse-primary: '#a6c8ff'
  secondary: '#5d5f5f'
  on-secondary: '#ffffff'
  secondary-container: '#dcdddd'
  on-secondary-container: '#5f6161'
  tertiary: '#833900'
  on-tertiary: '#ffffff'
  tertiary-container: '#a84c00'
  on-tertiary-container: '#ffe0d1'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#d5e3ff'
  primary-fixed-dim: '#a6c8ff'
  on-primary-fixed: '#001c3b'
  on-primary-fixed-variant: '#004787'
  secondary-fixed: '#e2e2e2'
  secondary-fixed-dim: '#c6c6c7'
  on-secondary-fixed: '#1a1c1c'
  on-secondary-fixed-variant: '#454747'
  tertiary-fixed: '#ffdbc9'
  tertiary-fixed-dim: '#ffb68d'
  on-tertiary-fixed: '#331200'
  on-tertiary-fixed-variant: '#763300'
  background: '#fcf9f8'
  on-background: '#1c1b1b'
  surface-variant: '#e5e2e1'
typography:
  display-sm:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '600'
    lineHeight: 24px
    letterSpacing: -0.01em
  body-lg:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  body-sm:
    fontFamily: Inter
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 18px
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.02em
  mono-sm:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 16px
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  unit: 4px
  container-padding: 24px
  item-gap: 8px
  stack-gap: 12px
  sidebar-width: 260px
  grid-gutter: 16px
---

## Brand & Style

The design system is engineered for high-performance desktop utility, prioritizing clarity, speed, and precision. It targets power users who require a professional environment that feels native to modern Windows 11 ecosystems while maintaining a more rigorous, organized structure.

The visual style is **Corporate / Modern** with a focus on **Tonal Layering**. It avoids decorative flourishes in favor of functional density. The emotional response is one of calm control—a "cockpit" experience where every pixel serves a purpose. The aesthetic is defined by crisp lines, structured grids, and a systematic approach to information density that reduces cognitive load during complex tasks.

## Colors

The palette is strictly functional, utilizing a range of neutral grays to establish hierarchy. 

- **Primary Blue (#0067C0):** Reserved exclusively for focus states, active indicators, and primary action buttons. It is a tool, not a decoration.
- **Neutral Scale:** Uses a range from Crisp White (#FFFFFF) for content surfaces to Deep Black (#1A1A1A) for high-contrast text. 
- **Surface Tones:** Backgrounds utilize a "Mica-inspired" light gray (#F9F9F9) to provide a sense of depth without the blur-heavy distraction of glassmorphism. 
- **Borders:** A consistent low-contrast stroke (#E5E5E5) is used to define boundaries in dense layouts.

## Typography

This design system utilizes **Inter** as a highly legible alternative to Segoe UI Variable for cross-platform precision, emphasizing a systematic, utilitarian feel. 

- **Density:** Font sizes are slightly smaller than standard web apps to accommodate professional desktop density (13px/14px base).
- **Functionality:** **JetBrains Mono** is introduced for technical strings, keyboard shortcuts, and monitor IDs to ensure character distinction.
- **Hierarchy:** Use FontWeight 600 for section headers and 400 for all primary data entries. Label styles use medium weight with slight tracking for increased legibility at small scales.

## Layout & Spacing

The layout follows a **Fixed Sidebar + Fluid Content** model typical of utility software. It uses a rigorous 4px base grid.

- **Grid:** Content areas use a 12-column fluid grid for dashboard views, with 16px gutters.
- **Margins:** Standard window padding is 24px, but internal component containers drop to 12px or 8px to maintain a dense, "pro" feel.
- **Responsiveness:** As the window shrinks, grid cards reflow from 4 columns to 2. The sidebar can collapse into an icon-only rail (64px width) to maximize workspace.
- **Alignment:** All elements must align to the 4px baseline. No "optical" centering that breaks the grid.

## Elevation & Depth

This design system avoids heavy shadows and deep stacking. Depth is communicated through **Low-contrast outlines** and subtle tonal shifts.

- **Base Level:** The application window background (#F9F9F9).
- **Surface Level:** Content cards and main panels use #FFFFFF with a 1px #E5E5E5 border.
- **Active Elevation:** When an item is dragged or prioritized, it receives a very soft, 4px blur shadow with 5% opacity. 
- **In-set Depth:** Search fields and input areas use a subtle 1px inner border or a slightly darker background (#F0F0F0) to appear "seated" within the surface.

## Shapes

The shape language is **Soft** but disciplined. 

- **Components:** Standard buttons, input fields, and tags use a 4px (0.25rem) corner radius.
- **Containers:** Large cards and main content areas use an 8px (0.5rem) radius.
- **Selection Indicators:** Active states in lists (sidebars) use a 4px radius for the highlight bar.
- **Icons:** Use a 2px stroke weight with rounded caps to match the UI's subtle corner radius.

## Components

- **Buttons:** Primary buttons use the Accent Blue. Ghost buttons (borderless) are preferred for secondary actions in toolbars to reduce visual noise.
- **Search Fields:** Full-width with a 1px border and a leading "Magnify" icon. Use `mono-sm` for the "Ctrl+K" shortcut hint aligned to the right.
- **List Rows:** 36px height for high density. Include a 16px icon slot, primary label, and trailing "status indicator" (e.g., a small green/gray dot for monitor connectivity).
- **Grid Cards:** Used for monitor or device previews. A 16:9 thumbnail area at the top, a 1px divider, and a metadata section at the bottom.
- **Segmented Controls:** Used for switching views (e.g., "List", "Grid", "Graph"). Flat design with a sliding background highlight for the active state.
- **Keyboard Shortcuts:** Styled as "KBD" tags using `mono-sm` typography, a light gray background, and a subtle bottom-border to simulate a physical key.
- **Monitor Indicators:** Small, high-contrast badges used in the corner of thumbnails to indicate resolution or refresh rate (e.g., "4K", "144Hz").