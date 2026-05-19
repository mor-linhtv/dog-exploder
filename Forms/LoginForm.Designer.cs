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
        components = new System.ComponentModel.Container();
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

        // pnlCard
        pnlCard.Location = new Point(60, 40);
        pnlCard.Size = new Size(360, 340);
        pnlCard.BackColor = Color.White;
        pnlCard.Paint += PnlCard_Paint;

        // picLogo
        picLogo.Location = new Point(156, 16);
        picLogo.Size = new Size(48, 48);
        picLogo.BackColor = Color.FromArgb(0xE2, 0xE2, 0xE2);
        picLogo.SizeMode = PictureBoxSizeMode.CenterImage;
        picLogo.Paint += PicLogo_Paint;

        // lblTitle
        lblTitle.Text = "Đăng nhập";
        lblTitle.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
        lblTitle.Location = new Point(0, 76);
        lblTitle.Size = new Size(360, 36);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;

        // lblSubtitle
        lblSubtitle.Text = "Chào mừng bạn quay lại với Dog Explorer";
        lblSubtitle.Font = new Font("Segoe UI", 9f);
        lblSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblSubtitle.Location = new Point(0, 116);
        lblSubtitle.Size = new Size(360, 20);
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;

        // lblUser
        lblUser.Text = "Tên đăng nhập";
        lblUser.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblUser.Location = new Point(20, 150);
        lblUser.Size = new Size(320, 18);

        // txtUsername
        txtUsername.Location = new Point(20, 170);
        txtUsername.Size = new Size(320, 26);
        txtUsername.Font = new Font("Segoe UI", 10f);
        txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        txtUsername.TextChanged += TxtUsername_TextChanged;

        // lblPass
        lblPass.Text = "Mật khẩu";
        lblPass.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblPass.Location = new Point(20, 204);
        lblPass.Size = new Size(320, 18);

        // txtPassword
        txtPassword.Location = new Point(20, 224);
        txtPassword.Size = new Size(320, 26);
        txtPassword.Font = new Font("Segoe UI", 10f);
        txtPassword.PlaceholderText = "Nhập mật khẩu";
        txtPassword.UseSystemPasswordChar = true;

        // chkRemember
        chkRemember.Text = "Ghi nhớ đăng nhập";
        chkRemember.Font = new Font("Segoe UI", 9f);
        chkRemember.Location = new Point(20, 260);
        chkRemember.Size = new Size(180, 22);

        // lnkForgot
        lnkForgot.Text = "Quên mật khẩu?";
        lnkForgot.LinkColor = Color.FromArgb(0x00, 0x78, 0xD4);
        lnkForgot.Font = new Font("Segoe UI", 9f);
        lnkForgot.Location = new Point(220, 261);
        lnkForgot.Size = new Size(120, 20);
        lnkForgot.TextAlign = ContentAlignment.MiddleRight;

        // btnLogin
        btnLogin.Text = "Đăng nhập";
        btnLogin.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        btnLogin.Location = new Point(20, 290);
        btnLogin.Size = new Size(320, 36);
        btnLogin.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Enabled = false;
        btnLogin.Click += BtnLogin_Click;

        pnlCard.Controls.AddRange(new Control[] { picLogo, lblTitle, lblSubtitle, lblUser, txtUsername, lblPass, txtPassword, chkRemember, lnkForgot, btnLogin });

        // LoginForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(480, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Dog Explorer — Đăng nhập";
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        Controls.Add(pnlCard);
        AcceptButton = btnLogin;
    }
}
