namespace Dog_Exploder.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlCard;
    private PictureBox picLogo;
    private Label lblTitle;
    private Label lblSubtitle;
    private Label lblUser;
    private TextBox txtUsername;
    private Label lblPass;
    private TextBox txtPassword;
    private CheckBox chkRemember;
    private LinkLabel lnkForgot;
    private Button btnLogin;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlCard = new Panel();
        picLogo = new PictureBox();
        lblTitle = new Label();
        lblSubtitle = new Label();
        lblUser = new Label();
        txtUsername = new TextBox();
        lblPass = new Label();
        txtPassword = new TextBox();
        chkRemember = new CheckBox();
        lnkForgot = new LinkLabel();
        btnLogin = new Button();
        pnlCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
        SuspendLayout();
        // 
        // pnlCard
        // 
        pnlCard.BackColor = Color.White;
        pnlCard.Controls.Add(picLogo);
        pnlCard.Controls.Add(lblTitle);
        pnlCard.Controls.Add(lblSubtitle);
        pnlCard.Controls.Add(lblUser);
        pnlCard.Controls.Add(txtUsername);
        pnlCard.Controls.Add(lblPass);
        pnlCard.Controls.Add(txtPassword);
        pnlCard.Controls.Add(chkRemember);
        pnlCard.Controls.Add(lnkForgot);
        pnlCard.Controls.Add(btnLogin);
        pnlCard.Location = new Point(60, 40);
        pnlCard.Name = "pnlCard";
        pnlCard.Size = new Size(360, 340);
        pnlCard.TabIndex = 0;
        pnlCard.Paint += PnlCard_Paint;
        // 
        // picLogo
        // 
        picLogo.BackColor = Color.FromArgb(226, 226, 226);
        picLogo.Location = new Point(156, 16);
        picLogo.Name = "picLogo";
        picLogo.Size = new Size(48, 48);
        picLogo.SizeMode = PictureBoxSizeMode.CenterImage;
        picLogo.TabIndex = 0;
        picLogo.TabStop = false;
        picLogo.Click += picLogo_Click;
        picLogo.Paint += PicLogo_Paint;
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblTitle.Location = new Point(0, 76);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(360, 36);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "Đăng nhập";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblSubtitle
        // 
        lblSubtitle.Font = new Font("Segoe UI", 9F);
        lblSubtitle.ForeColor = Color.FromArgb(64, 71, 82);
        lblSubtitle.Location = new Point(0, 116);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(360, 20);
        lblSubtitle.TabIndex = 2;
        lblSubtitle.Text = "Chào mừng bạn quay lại với Dog Explorer";
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblUser
        // 
        lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUser.Location = new Point(20, 150);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(320, 18);
        lblUser.TabIndex = 3;
        lblUser.Text = "Tên đăng nhập";
        // 
        // txtUsername
        // 
        txtUsername.Font = new Font("Segoe UI", 10F);
        txtUsername.Location = new Point(20, 170);
        txtUsername.Name = "txtUsername";
        txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        txtUsername.Size = new Size(320, 25);
        txtUsername.TabIndex = 4;
        txtUsername.TextChanged += TxtUsername_TextChanged;
        // 
        // lblPass
        // 
        lblPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPass.Location = new Point(20, 204);
        lblPass.Name = "lblPass";
        lblPass.Size = new Size(320, 18);
        lblPass.TabIndex = 5;
        lblPass.Text = "Mật khẩu";
        // 
        // txtPassword
        // 
        txtPassword.Font = new Font("Segoe UI", 10F);
        txtPassword.Location = new Point(20, 224);
        txtPassword.Name = "txtPassword";
        txtPassword.PlaceholderText = "Nhập mật khẩu";
        txtPassword.Size = new Size(320, 25);
        txtPassword.TabIndex = 6;
        txtPassword.UseSystemPasswordChar = true;
        // 
        // chkRemember
        // 
        chkRemember.Font = new Font("Segoe UI", 9F);
        chkRemember.Location = new Point(20, 260);
        chkRemember.Name = "chkRemember";
        chkRemember.Size = new Size(180, 22);
        chkRemember.TabIndex = 7;
        chkRemember.Text = "Ghi nhớ đăng nhập";
        // 
        // lnkForgot
        // 
        lnkForgot.Font = new Font("Segoe UI", 9F);
        lnkForgot.LinkColor = Color.FromArgb(0, 120, 212);
        lnkForgot.Location = new Point(220, 261);
        lnkForgot.Name = "lnkForgot";
        lnkForgot.Size = new Size(120, 20);
        lnkForgot.TabIndex = 8;
        lnkForgot.TabStop = true;
        lnkForgot.Text = "Quên mật khẩu?";
        lnkForgot.TextAlign = ContentAlignment.MiddleRight;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(0, 120, 212);
        btnLogin.Enabled = false;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(20, 290);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(320, 36);
        btnLogin.TabIndex = 9;
        btnLogin.Text = "Đăng nhập";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += BtnLogin_Click;
        // 
        // LoginForm
        // 
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(249, 249, 249);
        ClientSize = new Size(480, 420);
        Controls.Add(pnlCard);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Dog Explorer — Đăng nhập";
        pnlCard.ResumeLayout(false);
        pnlCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
        ResumeLayout(false);
    }
}
