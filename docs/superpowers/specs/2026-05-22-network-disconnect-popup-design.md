# Network Disconnect Popup — Design

**Date:** 2026-05-22
**Status:** Approved
**Related:** [Network Status Indicator](2026-05-22-network-status-indicator-design.md) — this feature builds on the existing `NetworkMonitorService` + `NetworkStatusBar` and replaces the prior YAGNI decision "no banners" with a popup-on-transition behavior.

## Problem

The existing status bar at the bottom of `MainForm` is passive — users only notice connectivity loss if they look down. When the network drops mid-session (e.g., Wi-Fi disconnect, VPN drop), the user keeps clicking and seeing generic API failures. We need a foreground signal at the moment connectivity is lost so the user can stop, diagnose, and recover.

## Decisions (from brainstorming)

- **Popup type:** Custom themed `Form`, not `MessageBox.Show`. Keeps brand consistency.
- **Trigger:** Only on `Online → Offline` transitions. App starting offline (`Unknown → Offline`) does NOT popup — the problem statement is "lost during use."
- **Frequency:** At most one offline modal per offline run. After user dismisses (or modal auto-closes on recovery), no re-show even if subsequent ticks remain offline. The recovery toast resets the "notified" flag so the next disconnect popups again.
- **Recovery:** On `Offline → Online`, if a modal was shown for the current offline run, show a lightweight toast "Đã kết nối lại với mạng" that auto-dismisses after 3 s. If the offline modal is still open, auto-close it before showing the toast.
- **Buttons on offline modal:** OK only. Service keeps polling in the background — no Retry button.

## Architecture

Three new types; one wiring change.

### 1. `Services/NetworkAlertCoordinator` (non-visual, `IDisposable`)

Owns the state machine and the two popup forms. Constructor takes the owner `Form` (used for `BeginInvoke` marshalling and as the popup owner) and the `NetworkMonitorService` instance. Subscribes to `monitor.StatusChanged` in its constructor.

State fields (UI-thread-only, no locking):

```csharp
private bool? _lastOnline;                 // null until first check
private bool _offlineDialogShownThisRun;   // true after modal shown, reset on recovery toast
private NetworkOfflineDialog? _offlineDialog;
```

Handler `OnStatusChanged` runs on the threadpool. It must call `_ownerForm.BeginInvoke(...)` to a private `ApplyState` method before touching any field or Form (same pattern as `Controls/NetworkStatusBar.OnStatusChanged`). Guard with `IsHandleCreated`/`IsDisposed` and catch `InvalidOperationException` for handle teardown races.

Transition table (executed inside `ApplyState` on UI thread):

| `_lastOnline` | `e.IsOnline` | Action |
|---|---|---|
| `null` | `true` | `_lastOnline = true`; no UI |
| `null` | `false` | `_lastOnline = false`; **no popup** (initial state, not "lost during use") |
| `true` | `false` | Construct `NetworkOfflineDialog`, wire `FormClosed` to clear `_offlineDialog`, set `_offlineDialogShownThisRun = true` and `_lastOnline = false` **before** calling `_offlineDialog.ShowDialog(_ownerForm)` (modal; state is mutated up-front so any re-entrant `ApplyState` calls during the nested message loop see consistent state) |
| `false` | `false` | No-op (already notified) |
| `false` | `true` | If `_offlineDialog != null`, call `_offlineDialog.Close()`; if `_offlineDialogShownThisRun`, instantiate + `Show(_ownerForm)` a `NetworkRecoveryToast`; reset `_offlineDialogShownThisRun = false`; `_lastOnline = true` |
| `true` | `true` | No-op |

`_offlineDialog.FormClosed` handler sets `_offlineDialog = null` so the coordinator forgets the reference whether close came from user OK or from the coordinator's own `Close()` call during recovery.

`Dispose()`:
- Unsubscribe `monitor.StatusChanged`.
- If `_offlineDialog != null && !_offlineDialog.IsDisposed`, `Close()` it.
- Idempotent (set a `_disposed` flag).

### 2. `Forms/NetworkOfflineDialog` (three-file split: `.cs`, `.Designer.cs`, `.resx`)

Modal form shown via `ShowDialog(owner)` — see Threading section for why this works with auto-close on recovery.

**Disposal note:** Forms shown via `ShowDialog` are NOT auto-disposed when closed (unlike `Show()` forms). The coordinator must `Dispose()` the dialog instance after `ShowDialog` returns. Recommended pattern in coordinator: `using var dialog = new NetworkOfflineDialog(); ... dialog.ShowDialog(_ownerForm);` — `using` handles disposal after the call returns, whether close came from user OK or programmatic `Close()`.

Designer properties:
- `FormBorderStyle = FixedDialog`
- `MaximizeBox = false`, `MinimizeBox = false`
- `ShowInTaskbar = false`
- `StartPosition = CenterParent`
- `Size = 420, 220`
- `BackColor = Theme.Surface`
- `AcceptButton = btnOk`
- `Text = "Mất kết nối mạng"`

Layout (top-to-bottom, 24 px outer padding):
- Header row: warning glyph (Segoe Fluent Icons `` or fallback Wingdings `!`) at left, label `"Mất kết nối mạng"` using `Theme.HeadlineMd` and `Theme.ErrorColor`.
- Body: two-line description label using `Theme.BodyMd` and `Theme.TextOnSurface`:
  - Line 1: `"Ứng dụng không thể kết nối đến dogapi.dog."`
  - Line 2: `"Vui lòng kiểm tra Wi-Fi, dây mạng hoặc firewall rồi thử lại."`
- Timestamp label `"Phát hiện lúc HH:mm:ss"` using `Theme.BodyMd` and `Theme.OnSurfaceVariant`.
- Bottom-right `btnOk`: width 96, height 32, `BackColor = Theme.Primary`, `ForeColor = Color.White`, `FlatStyle = Flat`, `Text = "OK"`, `DialogResult = OK`. Click handler calls `Close()`.

Public API in code-behind:
- `public NetworkOfflineDialog()` — parameterless, calls `InitializeComponent`.
- `public void SetDetectedAt(DateTime t)` — updates the timestamp label to `$"Phát hiện lúc {t:HH:mm:ss}"`.

The coordinator closes the dialog directly via `Close()` when recovery happens — no wrapper method needed.

### 3. `Forms/NetworkRecoveryToast` (three-file split)

Non-modal top-most form, auto-dismiss after 3 s.

Designer properties:
- `FormBorderStyle = None`
- `ShowInTaskbar = false`
- `TopMost = true`
- `StartPosition = Manual`
- `Size = 320, 56`
- `BackColor = Theme.Success`

Layout: check glyph (`` or `✓`) at left + label `"Đã kết nối lại với mạng"` using `Theme.BodyMd`, `ForeColor = Color.White`.

Custom paint (in code-behind `OnPaint`): draw rounded background using `Theme.RoundedRect(ClientRectangle, 8)`; set `this.Region = new Region(path)` once on `Load` so the corners are actually clipped (no border behind the rounded shape).

Position: in `OnLoad`, position relative to `Owner` (set via `Show(owner)`):
```csharp
var owner = this.Owner!;
int x = owner.Right - this.Width - 16;
int y = owner.Bottom - this.Height - 16 - 26; // 26 = NetworkStatusBar height
this.Location = new Point(x, y);
```
Recompute uses owner's screen coordinates, not client — use `owner.Location` + size or `owner.RectangleToScreen(owner.ClientRectangle)`. The toast is a top-level form, so its `Location` is in screen coordinates.

Auto-dismiss: `components`-owned `System.Windows.Forms.Timer` with `Interval = 3000`. Start in `OnLoad`. `Tick` handler stops the timer and calls `Close()`. Forms shown via `Show()` (not `ShowDialog`) auto-dispose on `Close()`, and the `Timer` is owned by `components` so it disposes with the form — no extra cleanup needed.

Public API:
- `public NetworkRecoveryToast()` — parameterless.

### Wiring in `MainForm`

Follow the existing `networkMonitor` pattern (field in Designer.cs, construct in code-behind):

1. **`Forms/MainForm.Designer.cs`** — add a field next to `networkMonitor`:
   ```csharp
   private NetworkAlertCoordinator? networkAlerts;
   ```
2. **`Forms/MainForm.cs` constructor** — after `networkStatusBar.Attach(networkMonitor);`:
   ```csharp
   networkAlerts = new NetworkAlertCoordinator(this, networkMonitor);
   ```
   Coordinator subscribes to `networkMonitor.StatusChanged` in its own constructor; no further wiring needed in `MainForm`.
3. **`Forms/MainForm.Designer.cs::Dispose(bool disposing)`** — dispose coordinator BEFORE the monitor so it unsubscribes before the monitor tears down its timer/HttpClient. Updated body:
   ```csharp
   protected override void Dispose(bool disposing)
   {
       if (disposing)
       {
           networkAlerts?.Dispose();
           networkMonitor?.Stop();
           networkMonitor?.Dispose();
           if (components != null) components.Dispose();
       }
       base.Dispose(disposing);
   }
   ```

## Threading model

- `NetworkMonitorService.StatusChanged` fires on threadpool (from `System.Threading.Timer` callback).
- `NetworkAlertCoordinator.OnStatusChanged` must marshal to UI thread via `_ownerForm.BeginInvoke`. Pattern mirrors `Controls/NetworkStatusBar.OnStatusChanged`:
  ```csharp
  private void OnStatusChanged(object? sender, NetworkStatusChangedEventArgs e)
  {
      if (!_ownerForm.IsHandleCreated || _ownerForm.IsDisposed) return;
      try { _ownerForm.BeginInvoke(new Action(() => ApplyState(e))); }
      catch (InvalidOperationException) { /* handle torn down */ }
  }
  ```
- All state fields are touched only inside `ApplyState` on the UI thread → no locking needed.
- **Why `ShowDialog(owner)` and how auto-close still works:** `ShowDialog` is the WinForms-idiomatic modal pattern (true input modality against the owner). Its nested message loop pumps posted messages, including `Control.BeginInvoke` callbacks — Timer ticks and `BeginInvoke` actions fire normally while a modal is open. So the recovery handler CAN run while `ShowDialog` is on the stack: it calls `_offlineDialog.Close()`, which unblocks the outer `ShowDialog` shortly after. To make this re-entrancy safe, the outer `ApplyState` mutates `_lastOnline`, `_offlineDialogShownThisRun`, and `_offlineDialog` BEFORE calling `ShowDialog`. The call to `ShowDialog` is the LAST statement in that branch — no code runs after it returns, so any state changes made by a re-entrant `ApplyState` (e.g., recovery) are preserved.

## Lifecycle scenarios

- **Logout while offline modal is open:** `MainForm.Dispose` → `_networkAlerts.Dispose` closes `_offlineDialog`. New `MainForm` instance after re-login starts with fresh state.
- **Recovery while offline modal still open:** handler closes modal, then shows toast. Modal's `FormClosed` clears `_offlineDialog` reference.
- **User clicks OK then network stays offline:** `_offlineDialogShownThisRun` stays true → no re-popup.
- **Initial state `Unknown → Offline → Online`:** no popup at offline, no toast at online (toast is gated on `_offlineDialogShownThisRun == true`).
- **Multiple toasts:** impossible — toast only shown when `_offlineDialogShownThisRun == true`, and that flag flips to false immediately after.

## YAGNI — explicitly NOT in scope

- No Retry button on the offline modal.
- No "Open Network Settings" button (no `ms-settings:network-status` launch).
- No "Don't show again" preference.
- No persistent offline log / history.
- No notification icon / tray balloon.
- No change to `NetworkStatusBar` — it keeps polling and rendering as today.
- No change to `NetworkMonitorService` — popup coordination layered on top of unchanged `StatusChanged` events.

## Acceptance

- Project builds with zero errors and zero new warnings.
- Online → unplug network: within ~15 s, modal "Mất kết nối mạng" appears centered over MainForm with timestamp, OK button.
- User clicks OK: modal closes. Keep network unplugged for another 30 s → no second modal appears.
- Re-plug network: within ~15 s, green toast "Đã kết nối lại với mạng" slides in at bottom-right above the status bar; disappears after 3 s.
- Unplug again: modal re-appears (this is a new offline run).
- Logout from MainForm during offline modal: app closes the modal cleanly, returns to login. Re-login → MainForm reloads → coordinator works again (no listener leak, no duplicate timer).
- Start app with network already off: status bar shows red Offline, no modal popup. Plug in network: status bar flips green, no recovery toast.
