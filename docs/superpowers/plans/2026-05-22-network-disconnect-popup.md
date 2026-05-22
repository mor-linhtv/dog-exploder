# Network Disconnect Popup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show a themed modal popup when the network goes from Online → Offline mid-session, plus a brief recovery toast when connectivity returns.

**Architecture:** Layer on top of the existing `NetworkMonitorService` (`StatusChanged` event, polls every 10 s) without modifying it. A new non-visual `NetworkAlertCoordinator` subscribes to the monitor, owns a small state machine, and shows two new Forms: `NetworkOfflineDialog` (modal, OK only) and `NetworkRecoveryToast` (non-modal, auto-dismiss 3 s). `MainForm` constructs the coordinator after `networkStatusBar.Attach(...)` and disposes it in `Dispose(bool)`.

**Tech Stack:** .NET 10 WinForms, GDI+ (`System.Drawing.Drawing2D` for rounded toast region), `System.Windows.Forms.Timer`.

**Validation:** No test project per CLAUDE.md. Each task verifies with `dotnet build` (zero errors, zero new warnings). Final task adds a manual smoke-test script you run yourself.

**Spec:** `docs/superpowers/specs/2026-05-22-network-disconnect-popup-design.md`

---

## File Map

| Action | File | Purpose |
|---|---|---|
| Create | `Forms/NetworkOfflineDialog.cs` | Code-behind: ctor + `SetDetectedAt` |
| Create | `Forms/NetworkOfflineDialog.Designer.cs` | Field decls + `InitializeComponent` + `Dispose` |
| Create | `Forms/NetworkOfflineDialog.resx` | Empty WinForms resx skeleton |
| Create | `Forms/NetworkRecoveryToast.cs` | Code-behind: ctor + OnLoad positioning + OnPaint + timer tick |
| Create | `Forms/NetworkRecoveryToast.Designer.cs` | Field decls + `InitializeComponent` + `Dispose` |
| Create | `Forms/NetworkRecoveryToast.resx` | Empty WinForms resx skeleton |
| Create | `Services/NetworkAlertCoordinator.cs` | State machine + Form lifecycle |
| Modify | `Forms/MainForm.Designer.cs` | Add `networkAlerts` field, dispose it before `networkMonitor` |
| Modify | `Forms/MainForm.cs` | Construct `NetworkAlertCoordinator(this, networkMonitor)` |

---

## Task 1: Create `NetworkOfflineDialog`

**Files:**
- Create: `Forms/NetworkOfflineDialog.Designer.cs`
- Create: `Forms/NetworkOfflineDialog.cs`
- Create: `Forms/NetworkOfflineDialog.resx`

- [ ] **Step 1: Create `Forms/NetworkOfflineDialog.Designer.cs`**

```csharp
using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

partial class NetworkOfflineDialog
{
    private System.ComponentModel.IContainer components = null;
    private Label lblIcon;
    private Label lblTitle;
    private Label lblBody;
    private Label lblDetectedAt;
    private Button btnOk;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblIcon = new Label();
        lblTitle = new Label();
        lblBody = new Label();
        lblDetectedAt = new Label();
        btnOk = new Button();
        SuspendLayout();

        // lblIcon
        lblIcon.AutoSize = false;
        lblIcon.Location = new Point(24, 24);
        lblIcon.Size = new Size(36, 36);
        lblIcon.Font = new Font("Segoe UI", 20f, FontStyle.Regular);
        lblIcon.ForeColor = Theme.ErrorColor;
        lblIcon.Text = "⚠";
        lblIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblTitle
        lblTitle.AutoSize = false;
        lblTitle.Location = new Point(68, 24);
        lblTitle.Size = new Size(328, 36);
        lblTitle.Font = Theme.HeadlineMd;
        lblTitle.ForeColor = Theme.ErrorColor;
        lblTitle.Text = "Mất kết nối mạng";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;

        // lblBody
        lblBody.AutoSize = false;
        lblBody.Location = new Point(24, 72);
        lblBody.Size = new Size(372, 48);
        lblBody.Font = Theme.BodyMd;
        lblBody.ForeColor = Theme.TextOnSurface;
        lblBody.Text = "Ứng dụng không thể kết nối đến dogapi.dog.\r\nVui lòng kiểm tra Wi-Fi, dây mạng hoặc firewall rồi thử lại.";

        // lblDetectedAt
        lblDetectedAt.AutoSize = false;
        lblDetectedAt.Location = new Point(24, 126);
        lblDetectedAt.Size = new Size(372, 20);
        lblDetectedAt.Font = Theme.BodyMd;
        lblDetectedAt.ForeColor = Theme.OnSurfaceVariant;
        lblDetectedAt.Text = "Phát hiện lúc --:--:--";

        // btnOk
        btnOk.Location = new Point(300, 162);
        btnOk.Size = new Size(96, 32);
        btnOk.Font = Theme.LabelLg;
        btnOk.BackColor = Theme.Primary;
        btnOk.ForeColor = Color.White;
        btnOk.FlatStyle = FlatStyle.Flat;
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Text = "OK";
        btnOk.DialogResult = DialogResult.OK;
        btnOk.UseVisualStyleBackColor = false;

        // Form
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Surface;
        AcceptButton = btnOk;
        Text = "Mất kết nối mạng";

        Controls.Add(lblIcon);
        Controls.Add(lblTitle);
        Controls.Add(lblBody);
        Controls.Add(lblDetectedAt);
        Controls.Add(btnOk);

        ResumeLayout(false);
    }
}
```

- [ ] **Step 2: Create `Forms/NetworkOfflineDialog.cs`**

```csharp
namespace Dog_Exploder.Forms;

public partial class NetworkOfflineDialog : Form
{
    public NetworkOfflineDialog()
    {
        InitializeComponent();
    }

    public void SetDetectedAt(DateTime t)
    {
        lblDetectedAt.Text = $"Phát hiện lúc {t:HH:mm:ss}";
    }
}
```

- [ ] **Step 3: Create `Forms/NetworkOfflineDialog.resx`** (empty WinForms resx — copy of the skeleton in existing `Forms/LoginForm.resx` is fine; minimal valid content below)

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
</root>
```

- [ ] **Step 4: Build to verify the dialog compiles**

Run: `dotnet build`
Expected: Build succeeded. 0 Error(s). No new warnings related to `NetworkOfflineDialog`.

If build fails: read the first error, fix the typo in the file mentioned, re-run. Don't proceed until clean.

- [ ] **Step 5: Commit**

```bash
git add Forms/NetworkOfflineDialog.cs Forms/NetworkOfflineDialog.Designer.cs Forms/NetworkOfflineDialog.resx
git commit -m "feat: add NetworkOfflineDialog modal form"
```

---

## Task 2: Create `NetworkRecoveryToast`

**Files:**
- Create: `Forms/NetworkRecoveryToast.Designer.cs`
- Create: `Forms/NetworkRecoveryToast.cs`
- Create: `Forms/NetworkRecoveryToast.resx`

- [ ] **Step 1: Create `Forms/NetworkRecoveryToast.Designer.cs`**

```csharp
using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

partial class NetworkRecoveryToast
{
    private System.ComponentModel.IContainer components = null;
    private Label lblIcon;
    private Label lblText;
    private System.Windows.Forms.Timer dismissTimer;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblIcon = new Label();
        lblText = new Label();
        dismissTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();

        // lblIcon
        lblIcon.AutoSize = false;
        lblIcon.Location = new Point(12, 0);
        lblIcon.Size = new Size(36, 56);
        lblIcon.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblIcon.ForeColor = Color.White;
        lblIcon.BackColor = Color.Transparent;
        lblIcon.Text = "✓";
        lblIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblText
        lblText.AutoSize = false;
        lblText.Location = new Point(54, 0);
        lblText.Size = new Size(254, 56);
        lblText.Font = Theme.BodyMd;
        lblText.ForeColor = Color.White;
        lblText.BackColor = Color.Transparent;
        lblText.Text = "Đã kết nối lại với mạng";
        lblText.TextAlign = ContentAlignment.MiddleLeft;

        // dismissTimer
        dismissTimer.Interval = 3000;
        dismissTimer.Tick += DismissTimer_Tick;

        // Form
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        ClientSize = new Size(320, 56);
        BackColor = Theme.Success;
        DoubleBuffered = true;

        Controls.Add(lblIcon);
        Controls.Add(lblText);

        ResumeLayout(false);
    }
}
```

- [ ] **Step 2: Create `Forms/NetworkRecoveryToast.cs`**

```csharp
using System.Drawing.Drawing2D;
using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

public partial class NetworkRecoveryToast : Form
{
    private const int StatusBarHeight = 26;
    private const int Margin = 16;

    public NetworkRecoveryToast()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        using (var path = Theme.RoundedRect(new Rectangle(0, 0, Width, Height), 8))
            Region = new Region(path);

        if (Owner != null)
        {
            int x = Owner.Right - Width - Margin;
            int y = Owner.Bottom - Height - Margin - StatusBarHeight;
            Location = new Point(x, y);
        }

        dismissTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.RoundedRect(new Rectangle(0, 0, Width, Height), 8);
        using var brush = new SolidBrush(Theme.Success);
        e.Graphics.FillPath(brush, path);
    }

    private void DismissTimer_Tick(object? sender, EventArgs e)
    {
        dismissTimer.Stop();
        Close();
    }
}
```

- [ ] **Step 3: Create `Forms/NetworkRecoveryToast.resx`** (empty WinForms resx skeleton — identical to the one from Task 1 Step 3)

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
</root>
```

- [ ] **Step 4: Build to verify**

Run: `dotnet build`
Expected: Build succeeded. 0 Error(s). No new warnings.

- [ ] **Step 5: Commit**

```bash
git add Forms/NetworkRecoveryToast.cs Forms/NetworkRecoveryToast.Designer.cs Forms/NetworkRecoveryToast.resx
git commit -m "feat: add NetworkRecoveryToast auto-dismiss form"
```

---

## Task 3: Create `NetworkAlertCoordinator`

**Files:**
- Create: `Services/NetworkAlertCoordinator.cs`

- [ ] **Step 1: Create the file**

```csharp
using Dog_Exploder.Forms;

namespace Dog_Exploder.Services;

internal sealed class NetworkAlertCoordinator : IDisposable
{
    private readonly Form _ownerForm;
    private readonly NetworkMonitorService _monitor;

    // State (UI-thread-only, no locking).
    private bool? _lastOnline;                    // null until first check
    private bool _offlineDialogShownThisRun;      // true after modal shown, reset on recovery toast
    private NetworkOfflineDialog? _offlineDialog; // tracked so recovery can auto-close it

    private bool _disposed;

    public NetworkAlertCoordinator(Form ownerForm, NetworkMonitorService monitor)
    {
        _ownerForm = ownerForm;
        _monitor = monitor;
        _monitor.StatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(object? sender, NetworkStatusChangedEventArgs e)
    {
        if (_disposed) return;
        if (!_ownerForm.IsHandleCreated || _ownerForm.IsDisposed) return;
        try
        {
            _ownerForm.BeginInvoke(new Action(() => ApplyState(e)));
        }
        catch (InvalidOperationException)
        {
            // Handle could be torn down between the check and BeginInvoke; ignore.
        }
    }

    private void ApplyState(NetworkStatusChangedEventArgs e)
    {
        if (_disposed || _ownerForm.IsDisposed) return;

        bool current = e.IsOnline;

        if (_lastOnline == null)
        {
            // Initial check — record state but don't popup. Starting offline is not
            // "lost during use".
            _lastOnline = current;
            return;
        }

        if (_lastOnline == true && !current)
        {
            // Online -> Offline: show the modal once per offline run.
            // Mutate state BEFORE ShowDialog so any re-entrant ApplyState call during
            // ShowDialog's nested message loop sees consistent state.
            _offlineDialogShownThisRun = true;
            _lastOnline = false;

            using var dialog = new NetworkOfflineDialog();
            dialog.SetDetectedAt(e.CheckedAt);
            dialog.FormClosed += (s, args) => _offlineDialog = null;
            _offlineDialog = dialog;
            dialog.ShowDialog(_ownerForm);
            // After ShowDialog returns: dialog is closed. `using` disposes it.
            // No code after this — recovery handler may have run re-entrantly and
            // already updated _offlineDialogShownThisRun / _lastOnline.
        }
        else if (_lastOnline == false && current)
        {
            // Offline -> Online: close modal (if still open), show recovery toast
            // (if the user was notified in this offline run).
            _offlineDialog?.Close();

            if (_offlineDialogShownThisRun)
            {
                var toast = new NetworkRecoveryToast();
                toast.Show(_ownerForm);
                // Toast is shown via Show() so it auto-disposes on Close().
            }

            _offlineDialogShownThisRun = false;
            _lastOnline = true;
        }
        // Else: false->false (already notified) or true->true (no change): no-op.
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _monitor.StatusChanged -= OnStatusChanged;
        if (_offlineDialog != null && !_offlineDialog.IsDisposed)
        {
            try { _offlineDialog.Close(); } catch { /* form may already be tearing down */ }
        }
    }
}
```

- [ ] **Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded. 0 Error(s). No new warnings.

- [ ] **Step 3: Commit**

```bash
git add Services/NetworkAlertCoordinator.cs
git commit -m "feat: add NetworkAlertCoordinator state machine"
```

---

## Task 4: Wire `NetworkAlertCoordinator` into `MainForm`

**Files:**
- Modify: `Forms/MainForm.Designer.cs` (add field + dispose call)
- Modify: `Forms/MainForm.cs` (construct in constructor)

- [ ] **Step 1: Add the field in `MainForm.Designer.cs`**

In `Forms/MainForm.Designer.cs`, find the line `private NetworkMonitorService networkMonitor;` (currently line 26) and add a new field directly below it:

Before:
```csharp
    private NetworkStatusBar networkStatusBar;
    private NetworkMonitorService networkMonitor;
```

After:
```csharp
    private NetworkStatusBar networkStatusBar;
    private NetworkMonitorService networkMonitor;
    private NetworkAlertCoordinator? networkAlerts;
```

- [ ] **Step 2: Update `Dispose(bool)` in `MainForm.Designer.cs` to dispose the coordinator first**

In `Forms/MainForm.Designer.cs`, replace the `Dispose(bool disposing)` body. Currently:

```csharp
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            networkMonitor?.Stop();
            networkMonitor?.Dispose();
            if (components != null) components.Dispose();
        }
        base.Dispose(disposing);
    }
```

Change to:

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

Order matters: coordinator must unsubscribe before `networkMonitor` disposes its `HttpClient` and `Timer`. Otherwise an in-flight `StatusChanged` callback could race against a half-disposed monitor.

- [ ] **Step 3: Construct the coordinator in `MainForm.cs`**

In `Forms/MainForm.cs`, locate the constructor and find the line `networkStatusBar.Attach(networkMonitor);` (currently line 26). Add one line directly below it:

Before:
```csharp
        networkStatusBar.Attach(networkMonitor);

        Load += MainForm_Load;
```

After:
```csharp
        networkStatusBar.Attach(networkMonitor);
        networkAlerts = new NetworkAlertCoordinator(this, networkMonitor);

        Load += MainForm_Load;
```

Note: `MainForm.cs` already imports `Dog_Exploder.Controls`. `NetworkAlertCoordinator` lives in `Dog_Exploder.Services`, which is already imported via `MainForm.Designer.cs` (line 2: `using Dog_Exploder.Services;`). Because both files share the `MainForm` `partial class`, that using is in scope. No new `using` needed in `MainForm.cs`.

- [ ] **Step 4: Build to verify**

Run: `dotnet build`
Expected: Build succeeded. 0 Error(s). No new warnings.

- [ ] **Step 5: Commit**

```bash
git add Forms/MainForm.cs Forms/MainForm.Designer.cs
git commit -m "feat: wire NetworkAlertCoordinator into MainForm"
```

---

## Task 5: Manual smoke test

No automated tests in this project. Verify the acceptance criteria from the spec by running the app and toggling network connectivity.

- [ ] **Step 1: Build + run the app**

Run: `dotnet run`
Expected: Login screen appears. Type any username + password, click Login. MainForm appears with the green "Online" status bar at the bottom within ~10 s.

- [ ] **Step 2: Test Online → Offline transition**

While the app is running and showing green Online:
- Disable your Wi-Fi adapter (or unplug the Ethernet cable, or use the host firewall to block `dogapi.dog`).
- Wait up to ~15 s (one 10 s poll + one 5 s HTTP timeout).

Expected:
- Status bar dot flips from green to red ("Offline").
- A themed modal popup titled "Mất kết nối mạng" appears centered over MainForm.
- Popup body shows the two-line Vietnamese description and "Phát hiện lúc HH:mm:ss" with the current time.
- Popup has a single blue "OK" button at the bottom-right.
- MainForm is non-interactive while the popup is open (clicks on sidebar/content do nothing — modal behavior).

- [ ] **Step 3: Test "no re-popup while still offline"**

Click "OK" to close the popup.
Wait another 30 s with the network still off.

Expected: status bar stays red Offline. No new popup appears.

- [ ] **Step 4: Test Offline → Online recovery toast**

Re-enable your Wi-Fi adapter (or plug the cable back / remove the firewall rule).
Wait up to ~15 s.

Expected:
- Status bar dot flips from red to green ("Online").
- A green rounded toast "Đã kết nối lại với mạng" appears at the bottom-right of MainForm, just above the status bar.
- The toast disappears on its own after ~3 s.

- [ ] **Step 5: Test "re-disconnect after recovery → popup re-appears"**

Disable the network again (same step as Step 2).
Wait ~15 s.

Expected: the modal "Mất kết nối mạng" popup appears again — a new offline run is treated as a new event.

- [ ] **Step 6: Test "auto-close modal on recovery"**

While the offline modal is still open (do NOT click OK), re-enable the network.
Wait ~15 s.

Expected:
- The offline modal closes automatically (without user interaction).
- The green recovery toast appears at the bottom-right and auto-dismisses after 3 s.

- [ ] **Step 7: Test "no popup if app started offline"**

Close the app. Disable the network. Run `dotnet run`. Log in.

Expected:
- Status bar shows red Offline within ~15 s.
- No modal popup appears (initial state, not "lost during use").

Re-enable the network.

Expected:
- Status bar flips green.
- No recovery toast (the popup never fired, so there's nothing to "recover" from from the UI's perspective).

- [ ] **Step 8: Test "logout + re-login still works"**

While online, click "Đăng xuất" on the sidebar. Log back in.
Disable the network.

Expected: offline popup appears as in Step 2. No listener leak, no duplicate timer, no double-popup.

- [ ] **Step 9: Test "logout while popup open closes cleanly"**

Trigger the offline popup (disable network, wait ~15 s, popup appears). Don't click OK. Click outside the modal — wait, that's blocked because it's modal. Instead, close the app via the window's X or the OS task switcher.

Expected: app exits cleanly with no exception dialog. (We close the popup in `Dispose`.)

- [ ] **Step 10: If smoke tests pass, no commit needed for this task**

The smoke test has no code changes. The feature is complete and matches the spec's acceptance criteria.

If any smoke test fails, do NOT mark this task complete. Report the failure: which step, what was expected, what actually happened. Treat it as a bug to debug (use `superpowers:systematic-debugging`).

---

## Self-review notes

- **Spec coverage:**
  - Custom themed modal popup ✓ (Task 1)
  - Recovery toast ✓ (Task 2)
  - Coordinator state machine ✓ (Task 3)
  - MainForm wiring ✓ (Task 4)
  - All 7 acceptance criteria ✓ (Task 5)
- **No placeholders:** every step shows code or exact commands. The resx file is duplicated by reference between tasks (Step 3 of Task 2 says "copy from Task 1") to avoid mismatch.
- **Type consistency:** `NetworkOfflineDialog`, `NetworkRecoveryToast`, `NetworkAlertCoordinator`, `networkAlerts`, `_lastOnline`, `_offlineDialogShownThisRun`, `_offlineDialog` — names used consistently across tasks.
