namespace Dog_Exploder.Controls;

partial class SidebarItem
{
    private System.ComponentModel.IContainer components = null;
    private Label lblText;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblText = new Label();

        lblText.Font = new Font("Segoe UI", 9.5f);
        lblText.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);
        lblText.Location = new Point(24, 10);
        lblText.Size = new Size(200, 20);
        lblText.Text = "Item";
        lblText.Cursor = Cursors.Hand;
        lblText.Click += (s, e) => OnClick(EventArgs.Empty);

        Size = new Size(240, 40);
        Cursor = Cursors.Hand;
        BackColor = Color.FromArgb(0xF3, 0xF3, 0xF3);
        Controls.Add(lblText);
        DoubleBuffered = true;
    }
}
