# Network Status Indicator — Design

**Date:** 2026-05-22
**Status:** Approved (proceeding to implementation per user goal "complete without errors")

## Problem

The app depends on internet connectivity for the Dog API (`https://dogapi.dog/`).
When the network is down, breed/image requests fail with generic exceptions
and the user has no at-a-glance way to tell whether the app is online. Add a
persistent indicator on `MainForm` so the user can see real-time connectivity.

## Decisions (from brainstorming)

- **Check scope:** Internet reachable — actually probe `dogapi.dog`, not just
  adapter up/down. Adapter-up is misleading when DNS/firewall blocks the API.
- **Placement:** Bottom status bar on `MainForm`, full width, ~26 px tall.
- **Cadence:** Background timer every 10 seconds. Visual indicator only —
  no banners, no disabling buttons.

## Architecture

Two new components:

### 1. `Services/NetworkMonitorService`

Lifecycle-managed instance (not static) owned by `MainForm`.

- Holds a `System.Threading.Timer` firing every 10 s on the threadpool, plus
  one immediate check on `Start()` so UI doesn't sit on "Unknown" for 10 s.
- Each tick: `HEAD https://dogapi.dog/api/v2/breeds?page[number]=1` with a
  5-second per-request timeout via a dedicated `HttpClient` (not shared with
  `DogApiService` — different timeout profile).
- State: `IsOnline`, `LastCheckedAt`, `LastError`.
- Event: `StatusChanged` fires every tick (UI decides whether to redraw).
- `Start()` / `Stop()` are idempotent. `Dispose()` stops + releases HttpClient.

Error handling: any exception → `IsOnline = false`, message stored in
`LastError` (shown as tooltip on the indicator). Timer callback wrapped in
try/catch so an unhandled exception never tears down the timer.

### 2. `Controls/NetworkStatusBar` (UserControl)

- Docks `Bottom` on `MainForm`, height 26 px, background `Theme.SidebarBg`.
- Renders `● <state> · checked HH:mm:ss` left-aligned, 12 px left padding.
- Colors: green `Theme.Success` for online, red `Theme.ErrorColor` for offline,
  gray `Theme.OnSurfaceVariant` for unknown (initial state).
- Subscribes to `StatusChanged`, marshals via `BeginInvoke` (checks
  `IsHandleCreated` first), updates label + dot color.
- Unsubscribes on `Dispose` to avoid leaks across logout cycles.

### Wiring on `MainForm`

- Construct monitor + status bar in `InitializeComponent`.
- `Controls.Add` order: pnlContent (Fill), pnlSidebar (Left), statusBar (Bottom).
  Bottom must dock before Fill in z-order so Fill yields the remaining area.
- `MainForm_Load` → `_monitor.Start()`.
- `Dispose(true)` → `_monitor.Stop()` + `_monitor.Dispose()`.

## Threading model

- Timer callback runs on threadpool — never touches UI directly.
- UI updates funnel through `Control.BeginInvoke`.
- `HttpClient` reused for the monitor's lifetime — no per-tick allocation.

## YAGNI — explicitly NOT in scope

- No retry/backoff (10 s polling absorbs blips).
- No disabling of network-dependent buttons.
- No `NetworkChange.NetworkAvailabilityChanged` subscription (redundant on
  top of 10 s polling that already validates real reachability).
- No state persistence/history.

## Acceptance

- Project builds with zero errors and zero new warnings.
- Status bar visible on every pane in `MainForm`.
- With network on: indicator shows green "Online" + recent timestamp.
- Unplug / block dogapi.dog: indicator flips to red "Offline" within ~15 s
  (first tick may still be in flight, 5 s timeout, then next tick).
- Logout + log back in: indicator resumes correctly (no listener leak,
  no duplicate timer).
