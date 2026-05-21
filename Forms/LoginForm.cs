using System.Drawing.Drawing2D;

namespace Dog_Exploder.Forms;

public partial class LoginForm : Form
{
    public string Username { get; private set; } = "";

    public LoginForm()
    {
        InitializeComponent();
    }

    private void TxtUsername_TextChanged(object? sender, EventArgs e)
    {
        btnLogin.Enabled = !string.IsNullOrWhiteSpace(txtUsername.Text);
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        Username = txtUsername.Text.Trim();
        if (string.IsNullOrEmpty(Username)) return;
        DialogResult = DialogResult.OK;
        this.Close();
    }

    private void PnlCard_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        using var pen = new Pen(Color.FromArgb(0xC0, 0xC7, 0xD4));
        g.DrawRectangle(pen, r);
    }

    private void PicLogo_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(0x00, 0x78, 0xD4));
        // simple paw glyph: 4 toes + heel — centered in 48×48
        g.FillEllipse(brush,  8, 17, 8, 8);
        g.FillEllipse(brush, 16, 11, 8, 8);
        g.FillEllipse(brush, 24, 11, 8, 8);
        g.FillEllipse(brush, 32, 17, 8, 8);
        g.FillEllipse(brush, 14, 25, 18, 12);
    }

    private void picLogo_Click(object sender, EventArgs e)
    {

    }
}
