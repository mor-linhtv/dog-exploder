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
        pnlSidebarBottom.Dock = DockStyle.Bottom;
        pnlSidebarBottom.Height = 80;
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
