using Dog_Exploder.UI;

namespace Dog_Exploder.Forms;

partial class NetworkRecoveryToast
{
    private System.ComponentModel.IContainer components = null;
    private Label lblIcon;
    private Label lblText;
    private System.Windows.Forms.Timer dismissTimer;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblIcon = new Label();
        lblText = new Label();
        dismissTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();

        // lblIcon
        lblIcon.AutoSize = false;
        lblIcon.Location = new Point(12, 0);
        lblIcon.Size = new Size(36, 56);
        lblIcon.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblIcon.ForeColor = Color.White;
        lblIcon.BackColor = Color.Transparent;
        lblIcon.Text = "✓";
        lblIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblText
        lblText.AutoSize = false;
        lblText.Location = new Point(54, 0);
        lblText.Size = new Size(254, 56);
        lblText.Font = Theme.BodyMd;
        lblText.ForeColor = Color.White;
        lblText.BackColor = Color.Transparent;
        lblText.Text = "Đã kết nối lại với mạng";
        lblText.TextAlign = ContentAlignment.MiddleLeft;

        // dismissTimer
        dismissTimer.Interval = 3000;
        dismissTimer.Tick += DismissTimer_Tick;

        // Form
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        ClientSize = new Size(320, 56);
        BackColor = Theme.Success;
        DoubleBuffered = true;

        Controls.Add(lblIcon);
        Controls.Add(lblText);

        ResumeLayout(false);
    }
}
