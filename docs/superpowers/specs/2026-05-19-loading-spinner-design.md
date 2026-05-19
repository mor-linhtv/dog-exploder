# Loading Spinner Design

**Date:** 2026-05-19
**Status:** Approved

## Overview

Add animated loading indicators to all screens that call APIs, replacing the current text-only loading states. A single reusable `LoadingSpinner` UserControl is built once and integrated into three locations.

## Affected Screens

| Screen | Current state | After |
|---|---|---|
| `BreedListControl` | Text "Đang tải dữ liệu..." in `pnlState` | Spinner + text in `pnlState` |
| `DeviceStatusControl` | Text "Đang kiểm tra thiết bị..." in `pnlState` | Spinner + text in `pnlState` |
| `BreedDetailControl` | Paw placeholder drawn in `PicImage_Paint` | Spinner overlay on `picImage` while image loads |

## LoadingSpinner Control

**File:** `Controls/LoadingSpinner.cs` (code-only, no Designer file)

**Mechanism:**
- UserControl with `BackColor = Color.Transparent`.
- A `System.Windows.Forms.Timer` fires every 30 ms.
- Each tick increments an internal `_angle` by 12° and calls `Invalidate()`.
- `OnPaint` draws 8 dots arranged in a circle. Each dot's opacity fades from 255 (lead dot) down to ~30 (trailing dot), creating a comet effect. Dot color: `#0078D4`.
- Default size: 40×40 px.

**Public API:**
- `Start()` — makes control visible, starts timer.
- `Stop()` — hides control, stops timer.

**Transparency note:** WinForms does not support true per-pixel transparency on `UserControl` inside a non-layered parent. To work around this, the spinner's `BackColor` is set to match the parent panel's background color (`Color.White` or `Color.FromArgb(0xF9, 0xF9, 0xF9)`) rather than true transparent. Each integration point sets `spinner.BackColor` to match its container.

## Integration: BreedListControl

- Add one `LoadingSpinner` instance to `pnlState` in `BreedListControl.Designer.cs`, centered above `lblState`.
- In `ShowState()`: call `spinner.Start()` before making `pnlState` visible.
- In `HideState()`: call `spinner.Stop()`.
- No changes to `LoadAsync()` or `ShowState()`/`HideState()` signatures.

## Integration: DeviceStatusControl

- Same pattern: add `LoadingSpinner` to `pnlState`, start in `ShowState()`, stop in `HideState()`.

## Integration: BreedDetailControl

- Add a `LoadingSpinner` field, sized 40×40.
- Remove the `PicImage_Paint` paw placeholder handler entirely — spinner replaces it.
- In `SetBreed()`: call `spinner.Start()` immediately (image starts loading).
- In `LoadImageAsync()`: call `spinner.Stop()` on the UI thread when image arrives or on cancellation/error.
- The spinner is added to the control's `Controls` collection (not `picImage`'s). It is positioned centered within `picImage` using `picImage.Left + (picImage.Width - 40) / 2` and `picImage.Top + (picImage.Height - 40) / 2`, calculated once in `InitializeComponent` and recalculated on `Resize` if `picImage` is anchored/dockable. `BringToFront()` is called after positioning.

## Error Handling

- If image load fails or is cancelled, `spinner.Stop()` is still called — the `picImage` area is left blank (no image, no spinner).
- Existing retry/error text in `BreedListControl` and `DeviceStatusControl` is unchanged.

## Out of Scope

- No changes to `LoginForm`.
- No changes to `ComingSoonControl`.
- No new NuGet packages.
