using System.ComponentModel;

namespace Dog_Exploder.Controls;

public partial class SidebarItem : UserControl
{
    private bool _active;
    private bool _hover;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Key { get; set; } = "";

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ItemText
    {
        get => lblText.Text;
        set => lblText.Text = value;
    }

    public override string Text
    {
        get => lblText.Text;
        set { base.Text = value; lblText.Text = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Active
    {
        get => _active;
        set { _active = value; Invalidate(); }
    }

    public SidebarItem()
    {
        InitializeComponent();
        MouseEnter += (s, e) => { _hover = true; Invalidate(); };
        MouseLeave += (s, e) => { _hover = false; Invalidate(); };
        lblText.MouseEnter += (s, e) => { _hover = true; Invalidate(); };
        lblText.MouseLeave += (s, e) => { _hover = false; Invalidate(); };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var bg = _active ? Color.FromArgb(0xE2, 0xE2, 0xE2)
              : _hover  ? Color.FromArgb(0xE8, 0xE8, 0xE8)
                        : Color.FromArgb(0xF3, 0xF3, 0xF3);
        using (var b = new SolidBrush(bg)) g.FillRectangle(b, ClientRectangle);

        if (_active)
        {
            using var bar = new SolidBrush(Color.FromArgb(0x00, 0x78, 0xD4));
            g.FillRectangle(bar, 0, 6, 3, Height - 12);
        }
        base.OnPaint(e);
    }
}
