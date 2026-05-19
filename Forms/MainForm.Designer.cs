using Dog_Exploder.Controls;

namespace Dog_Exploder.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlSidebar;
    private Panel pnlSidebarHeader;
    private Label lblBrandTitle;
    private Label lblBrandSubtitle;
    private Panel pnlSidebarBottom;
    private SidebarItem itemAllBreeds;
    private SidebarItem itemFavorites;
    private SidebarItem itemHistory;
    private SidebarItem itemComparison;
    private SidebarItem itemDevices;
    private SidebarItem itemSettings;
    private SidebarItem itemSupport;
    private Panel pnlContent;
    private Label lblGreeting;
    private Panel pnlLogoutSeparator;
    private Button btnLogout;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlSidebar = new Panel();
        pnlSidebarHeader = new Panel();
        lblBrandTitle = new Label();
        lblBrandSubtitle = new Label();
        pnlSidebarBottom = new Panel();
        itemAllBreeds = new SidebarItem { Key = "breeds", ItemText = "All Breeds" };
        itemFavorites = new SidebarItem { Key = "favorites", ItemText = "Favorites" };
        itemHistory = new SidebarItem { Key = "history", ItemText = "History" };
        itemComparison = new SidebarItem { Key = "comparison", ItemText = "Comparison" };
        itemDevices = new SidebarItem { Key = "devices", ItemText = "Device Status" };
        itemSettings = new SidebarItem { Key = "settings", ItemText = "Settings" };
        itemSupport = new SidebarItem { Key = "support", ItemText = "Support" };
        pnlLogoutSeparator = new Panel();
        btnLogout = new Button();
        pnlContent = new Panel();
        lblGreeting = new Label();

        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Width = 240;
        pnlSidebar.BackColor = Color.FromArgb(0xF3, 0xF3, 0xF3);

        pnlSidebarHeader.Dock = DockStyle.Top;
        pnlSidebarHeader.Height = 80;
        pnlSidebarHeader.Padding = new Padding(16);

        lblBrandTitle.Text = "Breed Explorer";
        lblBrandTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
        lblBrandTitle.Dock = DockStyle.Top;
        lblBrandTitle.Height = 22;

        lblBrandSubtitle.Text = "WinForms Edition";
        lblBrandSubtitle.Font = new Font("Segoe UI", 8.5f);
        lblBrandSubtitle.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblBrandSubtitle.Dock = DockStyle.Top;
        lblBrandSubtitle.Height = 18;

        pnlSidebarHeader.Controls.Add(lblBrandSubtitle);
        pnlSidebarHeader.Controls.Add(lblBrandTitle);

        itemSupport.Dock = DockStyle.Top;
        itemSettings.Dock = DockStyle.Top;
        pnlLogoutSeparator.Dock = DockStyle.Top;
        pnlLogoutSeparator.Height = 1;
        pnlLogoutSeparator.BackColor = Color.FromArgb(0xC0, 0xC7, 0xD4);

        btnLogout.Text = "Logout";
        btnLogout.Dock = DockStyle.Top;
        btnLogout.Height = 40;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.ForeColor = Color.FromArgb(0xC4, 0x2B, 0x1C);
        btnLogout.BackColor = Color.Transparent;
        btnLogout.Font = new Font("Segoe UI", 9.5f);
        btnLogout.TextAlign = ContentAlignment.MiddleLeft;
        btnLogout.Padding = new Padding(16, 0, 0, 0);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(0xFD, 0xE7, 0xE9);
        btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.Transparent;
        btnLogout.Click += BtnLogout_Click;

        pnlSidebarBottom.Dock = DockStyle.Bottom;
        pnlSidebarBottom.Height = 125;
        pnlSidebarBottom.Controls.Add(btnLogout);
        pnlSidebarBottom.Controls.Add(pnlLogoutSeparator);
        pnlSidebarBottom.Controls.Add(itemSupport);
        pnlSidebarBottom.Controls.Add(itemSettings);

        itemDevices.Dock = DockStyle.Top;
        itemComparison.Dock = DockStyle.Top;
        itemHistory.Dock = DockStyle.Top;
        itemFavorites.Dock = DockStyle.Top;
        itemAllBreeds.Dock = DockStyle.Top;

        pnlSidebar.Controls.Add(itemDevices);
        pnlSidebar.Controls.Add(itemComparison);
        pnlSidebar.Controls.Add(itemHistory);
        pnlSidebar.Controls.Add(itemFavorites);
        pnlSidebar.Controls.Add(itemAllBreeds);
        pnlSidebar.Controls.Add(pnlSidebarHeader);
        pnlSidebar.Controls.Add(pnlSidebarBottom);

        itemAllBreeds.Click  += (s, e) => ShowPane("breeds");
        itemFavorites.Click  += (s, e) => ShowPane("favorites");
        itemHistory.Click    += (s, e) => ShowPane("history");
        itemComparison.Click += (s, e) => ShowPane("comparison");
        itemDevices.Click    += (s, e) => ShowPane("devices");
        itemSettings.Click   += (s, e) => ShowPane("settings");
        itemSupport.Click    += (s, e) => ShowPane("support");

        pnlContent.Dock = DockStyle.Fill;
        pnlContent.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        pnlContent.Padding = new Padding(24, 12, 24, 24);

        lblGreeting.Dock = DockStyle.Top;
        lblGreeting.Height = 32;
        lblGreeting.TextAlign = ContentAlignment.MiddleRight;
        lblGreeting.Font = new Font("Segoe UI", 9.5f);
        lblGreeting.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 720);
        MinimumSize = new Size(900, 600);
        Text = "Dog Explorer";
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Controls.Add(pnlContent);
        Controls.Add(pnlSidebar);
    }
}
