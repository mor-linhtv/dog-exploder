namespace Dog_Exploder.Controls;

partial class BreedCard
{
    private System.ComponentModel.IContainer components = null;
    private PictureBox picImage;
    private Label lblName;
    private Label lblBadge;
    private Label lblDesc;

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _cts.Cancel(); _cts.Dispose(); if (components != null) components.Dispose(); }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        picImage = new PictureBox();
        lblName = new Label();
        lblBadge = new Label();
        lblDesc = new Label();

        picImage.Location = new Point(10, 10);
        picImage.Size = new Size(200, 130);
        picImage.SizeMode = PictureBoxSizeMode.Zoom;
        picImage.BackColor = Color.FromArgb(0xE8, 0xE8, 0xE8);
        picImage.Cursor = Cursors.Hand;
        picImage.Paint += PicImage_Paint;
        picImage.Click += Card_Click;

        lblName.Location = new Point(10, 148);
        lblName.Size = new Size(200, 22);
        lblName.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        lblName.ForeColor = Color.FromArgb(0x1A, 0x1C, 0x1C);
        lblName.Cursor = Cursors.Hand;
        lblName.Click += Card_Click;

        lblBadge.Location = new Point(10, 172);
        lblBadge.AutoSize = false;
        lblBadge.Size = new Size(200, 20);
        lblBadge.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblBadge.ForeColor = Color.FromArgb(0x00, 0x48, 0x83);
        lblBadge.TextAlign = ContentAlignment.MiddleLeft;
        lblBadge.Padding = new Padding(0);
        lblBadge.Cursor = Cursors.Hand;
        lblBadge.Click += Card_Click;
        lblBadge.Paint += LblBadge_Paint;

        lblDesc.Location = new Point(10, 196);
        lblDesc.Size = new Size(200, 54);
        lblDesc.Font = new Font("Segoe UI", 8.5f);
        lblDesc.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDesc.AutoEllipsis = true;
        lblDesc.Cursor = Cursors.Hand;
        lblDesc.Click += Card_Click;

        Size = new Size(220, 260);
        Margin = new Padding(8);
        BackColor = Color.White;
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        Click += Card_Click;

        Controls.Add(picImage);
        Controls.Add(lblName);
        Controls.Add(lblBadge);
        Controls.Add(lblDesc);
    }
}
