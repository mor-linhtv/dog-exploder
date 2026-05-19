---
name: Fluent WinForms Adaptation
colors:
  surface: '#f9f9f9'
  surface-dim: '#dadada'
  surface-bright: '#f9f9f9'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f3f3'
  surface-container: '#eeeeee'
  surface-container-high: '#e8e8e8'
  surface-container-highest: '#e2e2e2'
  on-surface: '#1a1c1c'
  on-surface-variant: '#404752'
  inverse-surface: '#2f3131'
  inverse-on-surface: '#f1f1f1'
  outline: '#717783'
  outline-variant: '#c0c7d4'
  surface-tint: '#0060ab'
  primary: '#005faa'
  on-primary: '#ffffff'
  primary-container: '#0078d4'
  on-primary-container: '#ffffff'
  inverse-primary: '#a3c9ff'
  secondary: '#0061a3'
  on-secondary: '#ffffff'
  secondary-container: '#5badff'
  on-secondary-container: '#003f6d'
  tertiary: '#974700'
  on-tertiary: '#ffffff'
  tertiary-container: '#bc5b00'
  on-tertiary-container: '#ffffff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#d3e3ff'
  primary-fixed-dim: '#a3c9ff'
  on-primary-fixed: '#001c39'
  on-primary-fixed-variant: '#004883'
  secondary-fixed: '#d1e4ff'
  secondary-fixed-dim: '#9ecaff'
  on-secondary-fixed: '#001d36'
  on-secondary-fixed-variant: '#00497d'
  tertiary-fixed: '#ffdbc8'
  tertiary-fixed-dim: '#ffb689'
  on-tertiary-fixed: '#311300'
  on-tertiary-fixed-variant: '#743500'
  background: '#f9f9f9'
  on-background: '#1a1c1c'
  surface-variant: '#e2e2e2'
typography:
  headline-lg:
    fontFamily: Inter
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-lg:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
  label-sm:
    fontFamily: Inter
    fontSize: 11px
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
  sidebar_width: 240px
  gutter: 16px
  margin_page: 24px
  stack_sm: 4px
  stack_md: 8px
  stack_lg: 16px
  control_height: 32px
---

## Brand & Style
The design system is a pragmatic, high-performance adaptation of modern Windows aesthetics tailored specifically for the technical constraints and user expectations of WinForms environments. It prioritizes clarity, reliability, and precision over decorative effects. 

The style is **Corporate / Modern** with a lean toward **High-Contrast Functionalism**. By stripping away resource-intensive blurs and complex shadows, the design system ensures snappy performance and pixel-perfect alignment. The emotional response is one of professional competence and systematic order, making it ideal for enterprise software, data-entry tools, and utility applications where efficiency is paramount.

## Colors
The palette is rooted in Windows Blue (#0078d4), utilizing a high-contrast logic to define control states. 

- **Primary**: Used for action-oriented elements and active indicators.
- **Surface & Neutrals**: The background relies on solid off-whites to distinguish between the sidebar and the main content area without needing shadows.
- **Interaction States**: Hover, Selected, and Pressed states use distinct, solid-fill neutral shifts. This ensures that users receive immediate visual feedback even in non-composited window environments.
- **Outlines**: Every interactive element must have a defined 1px border using the `outline` token to ensure separation against white backgrounds.

## Typography
This design system utilizes **Inter** for its superior legibility on low-DPI displays common in enterprise settings. The typographic hierarchy is disciplined, using weight rather than size to create emphasis where vertical space is limited. 

For WinForms implementation, ensure font rendering is set to `TextRenderingHint.ClearTypeGridFit`. Headlines should be reserved for page titles, while most UI interaction happens at the `body-md` and `label-md` levels.

## Layout & Spacing
The layout follows a **Fixed-Sidebar** model. Unlike responsive web apps, this system assumes a desktop environment where the Sidebar (Drawer) remains visible at a fixed width of 240px to provide persistent navigation with explicit text labels.

- **Grid**: Use a standard 8px baseline grid for all vertical stacking. 
- **Sidebar**: Fixed width with 8px internal padding for nav items.
- **Content Area**: 24px margins on all sides of the main container.
- **Controls**: Standardize input and button heights to 32px to maintain a dense but clickable interface.

## Elevation & Depth
Depth is achieved through **Low-Contrast Outlines** and **Tonal Layering** rather than shadows. 

1. **Base Layer**: The Sidebar uses a neutral background (#f3f3f3).
2. **Content Layer**: The main canvas is pure white (#ffffff).
3. **Interactive Layer**: Buttons and inputs sit on the canvas with a 1px solid border (#d1d1d1).
4. **Active State**: When a component is active (e.g., a selected tab or focused input), the border weight remains 1px but transitions to the Primary Blue (#0078d4) or a 2px interior focus ring.

Avoid all `DropShadow` properties. Use `ControlPaint.DrawBorder` with a single-pixel width for all container boundaries.

## Shapes
Geometry is strictly defined as **Soft (Round Four)**. 

- **Standard Controls**: Buttons, TextBoxes, and ComboBoxes use a 4px corner radius.
- **Large Containers**: Cards or Modals use an 8px corner radius.
- **Selection Indicators**: Use sharp 0px corners or a 2px vertical bar for the "Active" indicator in the sidebar to maintain a professional, architectural feel.

In WinForms, use `GraphicsPath` to create rounded rectangles for custom-drawn controls, ensuring that anti-aliasing is enabled for the border edges.

## Components

- **Buttons**: Solid primary fill with white text for "Primary" actions. White background with #d1d1d1 border for "Secondary". Hover state uses a 10% darken overlay.
- **Sidebar Items**: 40px height. Includes a 16px icon and an explicit text label. Active state shows a 3px wide primary-colored vertical bar on the extreme left.
- **Input Fields**: 1px border. On focus, the border changes to Primary Blue. Ensure a 4px internal padding for text.
- **Cards**: Use a white background with a 1px #d1d1d1 border. No shadow. Title should be `label-lg`.
- **Checkboxes/Radios**: 16px square/circle with a 1px border. Checkmark/Inner-dot uses Primary Blue.
- **Data Grids**: Use `body-md` for cell text. Header row should have a light neutral background (#f3f3f3) and a solid 1px bottom border. Cell selection should use `state_selected_hex`.