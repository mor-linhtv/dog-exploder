using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dog_Exploder.UI;

internal static class Theme
{
    public static readonly Color Primary           = Color.FromArgb(0x00, 0x78, 0xD4);
    public static readonly Color PrimaryDark       = Color.FromArgb(0x00, 0x5F, 0xAA);
    public static readonly Color Surface           = Color.FromArgb(0xF9, 0xF9, 0xF9);
    public static readonly Color Canvas            = Color.White;
    public static readonly Color SidebarBg         = Color.FromArgb(0xF3, 0xF3, 0xF3);
    public static readonly Color Border            = Color.FromArgb(0xC0, 0xC7, 0xD4);
    public static readonly Color BorderSoft        = Color.FromArgb(0xD1, 0xD1, 0xD1);
    public static readonly Color TextOnSurface     = Color.FromArgb(0x1A, 0x1C, 0x1C);
    public static readonly Color OnSurfaceVariant  = Color.FromArgb(0x40, 0x47, 0x52);
    public static readonly Color Success           = Color.FromArgb(0x19, 0x87, 0x54);
    public static readonly Color ErrorColor        = Color.FromArgb(0xBA, 0x1A, 0x1A);
    public static readonly Color Warning           = Color.FromArgb(0x97, 0x47, 0x00);
    public static readonly Color BadgeBg           = Color.FromArgb(0xD3, 0xE3, 0xFF);
    public static readonly Color BadgeFg           = Color.FromArgb(0x00, 0x48, 0x83);
    public static readonly Color BadgeDegradedBg   = Color.FromArgb(0xFD, 0xE7, 0xD3);
    public static readonly Color BadgeDegradedFg   = Color.FromArgb(0x97, 0x47, 0x00);
    public static readonly Color BadgeErrorBg      = Color.FromArgb(0xFF, 0xDA, 0xD6);
    public static readonly Color BadgeErrorFg      = Color.FromArgb(0x93, 0x00, 0x0A);
    public static readonly Color HoverBg           = Color.FromArgb(0xE8, 0xE8, 0xE8);
    public static readonly Color ActiveBg          = Color.FromArgb(0xE2, 0xE2, 0xE2);

    public static readonly Font HeadlineLg = new("Segoe UI", 20f, FontStyle.Bold);
    public static readonly Font HeadlineMd = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font BodyLg     = new("Segoe UI", 11f, FontStyle.Regular);
    public static readonly Font BodyMd     = new("Segoe UI", 9f,  FontStyle.Regular);
    public static readonly Font LabelLg    = new("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font LabelMd    = new("Segoe UI", 9f,  FontStyle.Bold);
    public static readonly Font LabelSm    = new("Segoe UI", 8f,  FontStyle.Regular);

    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void DrawRoundedBorder(Graphics g, Rectangle r, int radius, Color border, Color? fill = null)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = RoundedRect(new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1), radius);
        if (fill is { } f)
        {
            using var brush = new SolidBrush(f);
            g.FillPath(brush, path);
        }
        using var pen = new Pen(border, 1);
        g.DrawPath(pen, path);
    }
}
