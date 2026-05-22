using System.Drawing.Drawing2D;
using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

public partial class NetworkRecoveryToast : Form
{
    private const int StatusBarHeight = 26;
    private const int EdgeMargin = 16;

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
            int x = Owner.Right - Width - EdgeMargin;
            int y = Owner.Bottom - Height - EdgeMargin - StatusBarHeight;
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
