using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

partial class NetworkOfflineDialog
{
    private System.ComponentModel.IContainer components = null;
    private Label lblIcon;
    private Label lblTitle;
    private Label lblBody;
    private Label lblDetectedAt;
    private Button btnOk;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblIcon = new Label();
        lblTitle = new Label();
        lblBody = new Label();
        lblDetectedAt = new Label();
        btnOk = new Button();
        SuspendLayout();

        // lblIcon
        lblIcon.AutoSize = false;
        lblIcon.Location = new Point(24, 24);
        lblIcon.Size = new Size(36, 36);
        lblIcon.Font = new Font("Segoe UI", 20f, FontStyle.Regular);
        lblIcon.ForeColor = Theme.ErrorColor;
        lblIcon.Text = "⚠";
        lblIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblTitle
        lblTitle.AutoSize = false;
        lblTitle.Location = new Point(68, 24);
        lblTitle.Size = new Size(328, 36);
        lblTitle.Font = Theme.HeadlineMd;
        lblTitle.ForeColor = Theme.ErrorColor;
        lblTitle.Text = "Mất kết nối mạng";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;

        // lblBody
        lblBody.AutoSize = false;
        lblBody.Location = new Point(24, 72);
        lblBody.Size = new Size(372, 48);
        lblBody.Font = Theme.BodyMd;
        lblBody.ForeColor = Theme.TextOnSurface;
        lblBody.Text = "Ứng dụng không thể kết nối đến dogapi.dog.\r\nVui lòng kiểm tra Wi-Fi, dây mạng hoặc firewall rồi thử lại.";

        // lblDetectedAt
        lblDetectedAt.AutoSize = false;
        lblDetectedAt.Location = new Point(24, 126);
        lblDetectedAt.Size = new Size(372, 20);
        lblDetectedAt.Font = Theme.BodyMd;
        lblDetectedAt.ForeColor = Theme.OnSurfaceVariant;
        lblDetectedAt.Text = "Phát hiện lúc --:--:--";

        // btnOk
        btnOk.Location = new Point(300, 162);
        btnOk.Size = new Size(96, 32);
        btnOk.Font = Theme.LabelLg;
        btnOk.BackColor = Theme.Primary;
        btnOk.ForeColor = Color.White;
        btnOk.FlatStyle = FlatStyle.Flat;
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Text = "OK";
        btnOk.DialogResult = DialogResult.OK;
        btnOk.UseVisualStyleBackColor = false;

        // Form
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.Surface;
        AcceptButton = btnOk;
        Text = "Mất kết nối mạng";

        Controls.Add(lblIcon);
        Controls.Add(lblTitle);
        Controls.Add(lblBody);
        Controls.Add(lblDetectedAt);
        Controls.Add(btnOk);

        ResumeLayout(false);
    }
}
