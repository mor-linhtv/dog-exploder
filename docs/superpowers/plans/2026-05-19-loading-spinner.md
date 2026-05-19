# Loading Spinner Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an animated spinner to all screens that call APIs, replacing text-only loading states.

**Architecture:** A single `LoadingSpinner` control (pure GDI+, no Designer file) is created once and integrated at three sites: `BreedListControl.pnlState`, `DeviceStatusControl.pnlState`, and overlaid on `BreedDetailControl.picImage`.

**Tech Stack:** .NET 10 WinForms, GDI+ (`System.Drawing`), `System.Windows.Forms.Timer`

---

## File Map

| Action | File | Purpose |
|---|---|---|
| Create | `Controls/LoadingSpinner.cs` | Reusable animated spinner control |
| Modify | `Controls/BreedListControl.Designer.cs` | Add `_spinner` field + layout |
| Modify | `Controls/BreedListControl.cs` | Start/stop spinner in ShowState/HideState |
| Modify | `Controls/DeviceStatusControl.Designer.cs` | Add `_spinner` field + layout |
| Modify | `Controls/DeviceStatusControl.cs` | Start/stop spinner in ShowState/HideState |
| Modify | `Controls/BreedDetailControl.Designer.cs` | Add `_imageSpinner` field, remove PicImage_Paint hook |
| Modify | `Controls/BreedDetailControl.cs` | Remove PicImage_Paint, start/stop spinner per image load |

---

## Task 1: Create LoadingSpinner control

**Files:**
- Create: `Controls/LoadingSpinner.cs`

- [ ] **Step 1: Create the file**

```csharp
using System.Drawing.Drawing2D;

namespace Dog_Exploder.Controls;

public class LoadingSpinner : Control
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 30 };
    private float _angle;

    public LoadingSpinner()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint, true);
        Size = new Size(40, 40);
        Visible = false;
        _timer.Tick += (s, e) => { _angle = (_angle + 12f) % 360f; Invalidate(); };
    }

    public void Start() { Visible = true; _timer.Start(); }
    public void Stop()  { _timer.Stop(); Visible = false; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        const int count = 8;
        const float dotR = 3.5f;
        float orbit = Math.Min(Width, Height) / 2f - dotR - 2f;
        float cx = Width / 2f, cy = Height / 2f;

        for (int i = 0; i < count; i++)
        {
            double rad = (_angle + i * (360f / count)) * Math.PI / 180.0;
            float x = cx + orbit * (float)Math.Cos(rad);
            float y = cy + orbit * (float)Math.Sin(rad);
            int alpha = (int)(255 * (count - i) / (float)count);
            using var brush = new SolidBrush(Color.FromArgb(alpha, 0x00, 0x78, 0xD4));
            g.FillEllipse(brush, x - dotR, y - dotR, dotR * 2, dotR * 2);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }
}
```

- [ ] **Step 2: Build to verify no compile errors**

```
dotnet build Dog-Exploder.csproj
```

Expected: `Build succeeded` with 0 errors.

- [ ] **Step 3: Commit**

```
git add Controls/LoadingSpinner.cs
git commit -m "feat: add LoadingSpinner control"
```

---

## Task 2: Integrate spinner into BreedListControl

**Files:**
- Modify: `Controls/BreedListControl.Designer.cs`
- Modify: `Controls/BreedListControl.cs`

- [ ] **Step 1: Add `_spinner` field declaration to Designer.cs**

In the field declarations block at the top of `BreedListControl.Designer.cs`, add after `private Button btnRetry;`:

```csharp
private LoadingSpinner _spinner;
```

- [ ] **Step 2: Initialize and position spinner in InitializeComponent**

In `InitializeComponent()`, find the block where `pnlState` is configured. Replace the entire `pnlState` configuration block (from `pnlState.Dock = ...` to `pnlState.Controls.Add(lblState);` and the Resize handler) with:

```csharp
pnlState.Dock = DockStyle.Fill;
pnlState.BackColor = Color.White;
pnlState.Visible = false;

_spinner = new LoadingSpinner();
_spinner.BackColor = Color.White;

lblState.Text = "Đang tải...";
lblState.Font = new Font("Segoe UI", 11f);
lblState.TextAlign = ContentAlignment.TopCenter;
lblState.Size = new Size(400, 30);

btnRetry.Text = "Thử lại";
btnRetry.Visible = false;
btnRetry.Size = new Size(120, 32);
btnRetry.FlatStyle = FlatStyle.Flat;
btnRetry.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
btnRetry.ForeColor = Color.White;
btnRetry.FlatAppearance.BorderSize = 0;
btnRetry.Click += BtnRetry_Click;

pnlState.Controls.Add(_spinner);
pnlState.Controls.Add(lblState);
pnlState.Controls.Add(btnRetry);
pnlState.Resize += (s, e) =>
{
    _spinner.Location  = new Point((pnlState.Width - 40) / 2, pnlState.Height / 2 - 60);
    lblState.Location  = new Point((pnlState.Width - 400) / 2, pnlState.Height / 2 - 10);
    btnRetry.Location  = new Point((pnlState.Width - btnRetry.Width) / 2, pnlState.Height / 2 + 30);
};
```

- [ ] **Step 3: Update ShowState and HideState in BreedListControl.cs**

Replace the existing `ShowState` and `HideState` methods:

```csharp
private void ShowState(string text, bool showRetry)
{
    lblState.Text = text;
    btnRetry.Visible = showRetry;
    if (showRetry) _spinner.Stop(); else _spinner.Start();
    pnlState.Visible = true;
    pnlState.BringToFront();
}

private void HideState()
{
    _spinner.Stop();
    pnlState.Visible = false;
    pnlGrid.BringToFront();
}
```

- [ ] **Step 4: Build**

```
dotnet build Dog-Exploder.csproj
```

Expected: `Build succeeded`.

- [ ] **Step 5: Run and verify visually**

```
dotnet run --project Dog-Exploder.csproj
```

Navigate to the Breeds screen. On first load: spinner should appear centered in the content area, spinning blue dots. After data loads: spinner disappears, breed cards appear. Click "↻ Refresh": spinner appears again during reload.

- [ ] **Step 6: Commit**

```
git add Controls/BreedListControl.Designer.cs Controls/BreedListControl.cs
git commit -m "feat: add loading spinner to BreedListControl"
```

---

## Task 3: Integrate spinner into DeviceStatusControl

**Files:**
- Modify: `Controls/DeviceStatusControl.Designer.cs`
- Modify: `Controls/DeviceStatusControl.cs`

- [ ] **Step 1: Add `_spinner` field declaration**

In the field declarations block of `DeviceStatusControl.Designer.cs`, add after `private Label lblState;`:

```csharp
private LoadingSpinner _spinner;
```

- [ ] **Step 2: Update pnlState configuration in InitializeComponent**

Replace the existing `pnlState` + `lblState` configuration block with:

```csharp
pnlState.Dock = DockStyle.Fill;
pnlState.Visible = false;
pnlState.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);

_spinner = new LoadingSpinner();
_spinner.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);

lblState.Text = "Đang kiểm tra thiết bị...";
lblState.TextAlign = ContentAlignment.TopCenter;
lblState.Font = new Font("Segoe UI", 11f);
lblState.Size = new Size(400, 30);

pnlState.Controls.Add(_spinner);
pnlState.Controls.Add(lblState);
pnlState.Resize += (s, e) =>
{
    _spinner.Location = new Point((pnlState.Width - 40) / 2, pnlState.Height / 2 - 50);
    lblState.Location = new Point((pnlState.Width - 400) / 2, pnlState.Height / 2 + 0);
};
```

- [ ] **Step 3: Update ShowState and HideState in DeviceStatusControl.cs**

Replace the existing `ShowState` and `HideState` methods:

```csharp
private void ShowState(string text)
{
    lblState.Text = text;
    _spinner.Start();
    pnlState.Visible = true;
    pnlState.BringToFront();
}

private void HideState()
{
    _spinner.Stop();
    pnlState.Visible = false;
    pnlDevices.BringToFront();
}
```

- [ ] **Step 4: Build**

```
dotnet build Dog-Exploder.csproj
```

Expected: `Build succeeded`.

- [ ] **Step 5: Run and verify visually**

Navigate to the Devices screen. Spinner should appear centered while devices are being enumerated. After load: spinner disappears, device cards appear. Click "↻ Refresh All": spinner appears again.

- [ ] **Step 6: Commit**

```
git add Controls/DeviceStatusControl.Designer.cs Controls/DeviceStatusControl.cs
git commit -m "feat: add loading spinner to DeviceStatusControl"
```

---

## Task 4: Integrate spinner into BreedDetailControl

**Files:**
- Modify: `Controls/BreedDetailControl.Designer.cs`
- Modify: `Controls/BreedDetailControl.cs`

- [ ] **Step 1: Add `_imageSpinner` field declaration**

In the field declarations block of `BreedDetailControl.Designer.cs`, add after `private Label lblCareBody;`:

```csharp
private LoadingSpinner _imageSpinner;
```

- [ ] **Step 2: Wire up spinner in InitializeComponent**

In `InitializeComponent()`, find where `picImage` is configured. It ends with `picImage.Paint += PicImage_Paint;`.

Remove the line:
```csharp
picImage.Paint += PicImage_Paint;
```

Then after the `pnlHeader.Controls.Add(picImage);` line, add:

```csharp
_imageSpinner = new LoadingSpinner();
_imageSpinner.Size = new Size(40, 40);
_imageSpinner.Location = new Point(
    picImage.Left + (picImage.Width  - 40) / 2,
    picImage.Top  + (picImage.Height - 40) / 2);
_imageSpinner.BackColor = picImage.BackColor;
pnlHeader.Controls.Add(_imageSpinner);
_imageSpinner.BringToFront();
```

- [ ] **Step 3: Update BreedDetailControl.cs**

Remove the entire `PicImage_Paint` method:

```csharp
// DELETE this entire method:
private void PicImage_Paint(object? sender, PaintEventArgs e)
{
    if (picImage.Image != null) return;
    var g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;
    using var brush = new SolidBrush(Color.FromArgb(0xC0, 0xC7, 0xD4));
    var cx = picImage.Width / 2; var cy = picImage.Height / 2;
    g.FillEllipse(brush, cx - 22, cy - 6, 10, 10);
    g.FillEllipse(brush, cx - 10, cy - 14, 10, 10);
    g.FillEllipse(brush, cx + 2,  cy - 14, 10, 10);
    g.FillEllipse(brush, cx + 14, cy - 6, 10, 10);
    g.FillEllipse(brush, cx - 12, cy + 6, 22, 14);
}
```

In `SetBreed()`, after the line `_imageCts = new CancellationTokenSource();`, add:

```csharp
_imageSpinner.Start();
```

Replace the existing `LoadImageAsync` method with:

```csharp
private async Task LoadImageAsync(string name, CancellationToken ct)
{
    try
    {
        var img = await DogImageService.GetImageAsync(name, ct);
        if (ct.IsCancellationRequested || IsDisposed) return;
        BeginInvoke(() =>
        {
            if (IsDisposed) return;
            if (img != null) picImage.Image = img;
            _imageSpinner.Stop();
        });
    }
    catch
    {
        if (!IsDisposed) BeginInvoke(() => { if (!IsDisposed) _imageSpinner.Stop(); });
    }
}
```

- [ ] **Step 4: Build**

```
dotnet build Dog-Exploder.csproj
```

Expected: `Build succeeded`.

- [ ] **Step 5: Run and verify visually**

Navigate to Breeds, click any breed card. In the detail view: the grey image box shows a spinner while the photo loads. Once the photo appears, spinner disappears. Click "← Back to List", click another breed — spinner should appear again for the new photo.

- [ ] **Step 6: Commit**

```
git add Controls/BreedDetailControl.Designer.cs Controls/BreedDetailControl.cs
git commit -m "feat: add loading spinner to BreedDetailControl image"
```
