namespace Dog_Exploder.Controls;

partial class DeviceCard
{
    private System.ComponentModel.IContainer components = null;
    private Label lblName;
    private Label lblBadge;
    private Label lblCategory;
    private Label lblDetail;
    private Label lblChecked;
    private RadioButton rbSelect;
    private Button btnCheck;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblName = new Label();
        lblBadge = new Label();
        lblCategory = new Label();
        lblDetail = new Label();
        lblChecked = new Label();
        rbSelect = new RadioButton();
        btnCheck = new Button();

        lblName.Location = new Point(16, 14);
        lblName.Size = new Size(230, 22);
        lblName.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        lblName.AutoEllipsis = true;

        lblBadge.Location = new Point(252, 14);
        lblBadge.AutoSize = false;
        lblBadge.Size = new Size(100, 22);
        lblBadge.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblBadge.TextAlign = ContentAlignment.MiddleCenter;
        lblBadge.Padding = new Padding(8, 2, 8, 2);
        lblBadge.Paint += LblBadge_Paint;

        lblCategory.Location = new Point(16, 44);
        lblCategory.Size = new Size(330, 18);
        lblCategory.Font = new Font("Segoe UI", 9f);
        lblCategory.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        lblDetail.Location = new Point(16, 66);
        lblDetail.Size = new Size(330, 80);
        lblDetail.Font = new Font("Segoe UI", 9f);
        lblDetail.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDetail.AutoEllipsis = true;

        lblChecked.Location = new Point(16, 150);
        lblChecked.Size = new Size(330, 18);
        lblChecked.Font = new Font("Segoe UI", 8.5f);
        lblChecked.ForeColor = Color.FromArgb(0x71, 0x77, 0x83);

        rbSelect.Text = "Chọn để export";
        rbSelect.Location = new Point(16, 178);
        rbSelect.Size = new Size(160, 22);
        rbSelect.Font = new Font("Segoe UI", 9f);

        btnCheck.Text = "Check Connection";
        btnCheck.Location = new Point(196, 175);
        btnCheck.Size = new Size(150, 28);
        btnCheck.FlatStyle = FlatStyle.Flat;
        btnCheck.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnCheck.BackColor = Color.White;
        btnCheck.Click += BtnCheck_Click;

        Size = new Size(360, 220);
        Margin = new Padding(8);
        BackColor = Color.White;
        DoubleBuffered = true;

        Controls.Add(lblName);
        Controls.Add(lblBadge);
        Controls.Add(lblCategory);
        Controls.Add(lblDetail);
        Controls.Add(lblChecked);
        Controls.Add(rbSelect);
        Controls.Add(btnCheck);
    }
}
