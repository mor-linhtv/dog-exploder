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
