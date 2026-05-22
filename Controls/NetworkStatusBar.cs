using System.Drawing.Drawing2D;
using Dog_Exploder.Services;
using Dog_Exploder.UI;

namespace Dog_Exploder.Controls;

internal sealed class NetworkStatusBar : Control
{
    private NetworkMonitorService? _monitor;
    private readonly ToolTip _tip = new();

    private enum VisualState { Unknown, Online, Offline }
    private VisualState _state = VisualState.Unknown;
    private string _text = "Checking network…";

    public NetworkStatusBar()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw, true);
        Height = 26;
        Font = new Font("Segoe UI", 9f);
        BackColor = Theme.SidebarBg;
        ForeColor = Theme.TextOnSurface;
    }

    public void Attach(NetworkMonitorService monitor)
    {
        if (_monitor != null) _monitor.StatusChanged -= OnStatusChanged;
        _monitor = monitor;
        _monitor.StatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(object? sender, NetworkStatusChangedEventArgs e)
    {
        if (!IsHandleCreated || IsDisposed) return;
        try
        {
            BeginInvoke(new Action(() => ApplyState(e)));
        }
        catch (InvalidOperationException)
        {
            // Handle could be torn down between the check and BeginInvoke; ignore.
        }
    }

    private void ApplyState(NetworkStatusChangedEventArgs e)
    {
        _state = e.IsOnline ? VisualState.Online : VisualState.Offline;
        var label = e.IsOnline ? "Online" : "Offline";
        _text = $"{label}  ·  checked {e.CheckedAt:HH:mm:ss}";
        _tip.SetToolTip(this, e.IsOnline
            ? $"Connected to dogapi.dog at {e.CheckedAt:HH:mm:ss}"
            : $"Cannot reach dogapi.dog: {e.Error ?? "unknown error"}");
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (var border = new Pen(Theme.Border))
            g.DrawLine(border, 0, 0, Width, 0);

        var dotColor = _state switch
        {
            VisualState.Online  => Theme.Success,
            VisualState.Offline => Theme.ErrorColor,
            _                    => Theme.OnSurfaceVariant
        };

        const int dotDiameter = 10;
        int dotX = 14;
        int dotY = (Height - dotDiameter) / 2;
        using (var b = new SolidBrush(dotColor))
            g.FillEllipse(b, dotX, dotY, dotDiameter, dotDiameter);

        var textRect = new Rectangle(dotX + dotDiameter + 8, 0, Width - (dotX + dotDiameter + 16), Height);
        TextRenderer.DrawText(g, _text, Font, textRect, ForeColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_monitor != null) _monitor.StatusChanged -= OnStatusChanged;
            _tip.Dispose();
        }
        base.Dispose(disposing);
    }
}
