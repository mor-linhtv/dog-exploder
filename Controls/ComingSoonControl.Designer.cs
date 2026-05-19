namespace Dog_Exploder.Controls;

partial class ComingSoonControl
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitle;
    private Label lblSubtitle;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblTitle = new Label();
        lblSubtitle = new Label();

        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.Text = "Tính năng đang được phát triển";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Height = 40;
        lblTitle.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);

        lblSubtitle.Font = new Font("Segoe UI", 9f);
        lblSubtitle.Text = "Tab này hiện chưa khả dụng trong bản demo.";
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitle.Dock = DockStyle.Top;
        lblSubtitle.Height = 24;
        lblSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        Controls.Add(lblSubtitle);
        Controls.Add(lblTitle);
        Padding = new Padding(0, 200, 0, 0);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
