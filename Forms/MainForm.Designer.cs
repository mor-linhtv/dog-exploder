using Dog_Exploder.Controls;
using Dog_Exploder.Services;

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
    private Panel pnlContent;
    private Label lblGreeting;
    private Panel pnlLogoutSeparator;
    private Button btnLogout;
    private NetworkStatusBar networkStatusBar;
    private NetworkMonitorService networkMonitor;
    private NetworkAlertCoordinator networkAlerts;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            networkAlerts?.Dispose();
            networkMonitor?.Stop();
            networkMonitor?.Dispose();
            if (components != null) components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlSidebar = new Panel();
        itemAllBreeds = new SidebarItem();
        pnlSidebarHeader = new Panel();
        lblBrandSubtitle = new Label();
        lblBrandTitle = new Label();
        pnlSidebarBottom = new Panel();
        btnLogout = new Button();
        pnlLogoutSeparator = new Panel();
        itemSettings = new SidebarItem();
        itemFavorites = new SidebarItem();
        itemHistory = new SidebarItem();
        itemComparison = new SidebarItem();
        itemDevices = new SidebarItem();
        pnlContent = new Panel();
        lblGreeting = new Label();
        networkStatusBar = new NetworkStatusBar();
        networkMonitor = new NetworkMonitorService();
        pnlSidebar.SuspendLayout();
        pnlSidebarHeader.SuspendLayout();
        pnlSidebarBottom.SuspendLayout();
        SuspendLayout();
        // 
        // pnlSidebar
        // 
        pnlSidebar.BackColor = Color.FromArgb(243, 243, 243);
        pnlSidebar.Controls.Add(itemAllBreeds);
        pnlSidebar.Controls.Add(pnlSidebarHeader);
        pnlSidebar.Controls.Add(pnlSidebarBottom);
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Location = new Point(0, 0);
        pnlSidebar.Name = "pnlSidebar";
        pnlSidebar.Size = new Size(240, 694);
        pnlSidebar.TabIndex = 1;
        // 
        // itemAllBreeds
        // 
        itemAllBreeds.BackColor = Color.FromArgb(243, 243, 243);
        itemAllBreeds.Dock = DockStyle.Top;
        itemAllBreeds.Location = new Point(0, 80);
        itemAllBreeds.Name = "itemAllBreeds";
        itemAllBreeds.Size = new Size(240, 40);
        itemAllBreeds.Text = "All Breeds";
        itemAllBreeds.TabIndex = 0;
        // 
        // pnlSidebarHeader
        // 
        pnlSidebarHeader.Controls.Add(lblBrandSubtitle);
        pnlSidebarHeader.Controls.Add(lblBrandTitle);
        pnlSidebarHeader.Dock = DockStyle.Top;
        pnlSidebarHeader.Location = new Point(0, 0);
        pnlSidebarHeader.Name = "pnlSidebarHeader";
        pnlSidebarHeader.Padding = new Padding(16);
        pnlSidebarHeader.Size = new Size(240, 80);
        pnlSidebarHeader.TabIndex = 1;
        // 
        // lblBrandSubtitle
        // 
        lblBrandSubtitle.Dock = DockStyle.Top;
        lblBrandSubtitle.Font = new Font("Segoe UI", 8.5F);
        lblBrandSubtitle.ForeColor = Color.FromArgb(64, 71, 82);
        lblBrandSubtitle.Location = new Point(16, 38);
        lblBrandSubtitle.Name = "lblBrandSubtitle";
        lblBrandSubtitle.Size = new Size(208, 18);
        lblBrandSubtitle.TabIndex = 0;
        lblBrandSubtitle.Text = "WinForms Edition";
        lblBrandSubtitle.Click += lblBrandSubtitle_Click;
        // 
        // lblBrandTitle
        // 
        lblBrandTitle.Dock = DockStyle.Top;
        lblBrandTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblBrandTitle.Location = new Point(16, 16);
        lblBrandTitle.Name = "lblBrandTitle";
        lblBrandTitle.Size = new Size(208, 22);
        lblBrandTitle.TabIndex = 1;
        lblBrandTitle.Text = "Breed Explorer";
        // 
        // pnlSidebarBottom
        // 
        pnlSidebarBottom.Controls.Add(btnLogout);
        pnlSidebarBottom.Controls.Add(pnlLogoutSeparator);
        //pnlSidebarBottom.Controls.Add(itemSupport);
        pnlSidebarBottom.Controls.Add(itemSettings);
        pnlSidebarBottom.AutoSize = true;
        pnlSidebarBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        pnlSidebarBottom.Dock = DockStyle.Bottom;
        pnlSidebarBottom.Name = "pnlSidebarBottom";
        pnlSidebarBottom.TabIndex = 2;
        // 
        // btnLogout
        // 
        btnLogout.BackColor = Color.Transparent;
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.Dock = DockStyle.Top;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 9.5F);
        btnLogout.ForeColor = Color.FromArgb(196, 43, 28);
        btnLogout.Location = new Point(0, 81);
        btnLogout.Name = "btnLogout";
        btnLogout.Padding = new Padding(16, 0, 0, 0);
        btnLogout.Size = new Size(240, 40);
        btnLogout.TabIndex = 0;
        btnLogout.Text = "Logout";
        btnLogout.TextAlign = ContentAlignment.MiddleLeft;
        btnLogout.UseVisualStyleBackColor = false;
        btnLogout.Click += BtnLogout_Click;
        // 
        // pnlLogoutSeparator
        // 
        pnlLogoutSeparator.BackColor = Color.FromArgb(192, 199, 212);
        pnlLogoutSeparator.Dock = DockStyle.Top;
        pnlLogoutSeparator.Location = new Point(0, 80);
        pnlLogoutSeparator.Name = "pnlLogoutSeparator";
        pnlLogoutSeparator.Size = new Size(240, 1);
        pnlLogoutSeparator.TabIndex = 1;
        // 
        // itemSettings
        // 
        itemSettings.BackColor = Color.FromArgb(243, 243, 243);
        itemSettings.Dock = DockStyle.Top;
        itemSettings.Location = new Point(0, 0);
        itemSettings.Name = "itemSettings";
        itemSettings.Size = new Size(240, 40);
        itemSettings.Text = "Settings";
        itemSettings.TabIndex = 3;
        // 
        // itemFavorites
        // 
        itemFavorites.BackColor = Color.FromArgb(243, 243, 243);
        itemFavorites.Dock = DockStyle.Top;
        itemFavorites.Location = new Point(0, 0);
        itemFavorites.Name = "itemFavorites";
        itemFavorites.Size = new Size(240, 40);
        itemFavorites.TabIndex = 0;
        // 
        // itemHistory
        // 
        itemHistory.BackColor = Color.FromArgb(243, 243, 243);
        itemHistory.Dock = DockStyle.Top;
        itemHistory.Location = new Point(0, 0);
        itemHistory.Name = "itemHistory";
        itemHistory.Size = new Size(240, 40);
        itemHistory.TabIndex = 0;
        // 
        // itemComparison
        // 
        itemComparison.BackColor = Color.FromArgb(243, 243, 243);
        itemComparison.Dock = DockStyle.Top;
        itemComparison.Location = new Point(0, 0);
        itemComparison.Name = "itemComparison";
        itemComparison.Size = new Size(240, 40);
        itemComparison.TabIndex = 0;
        // 
        // itemDevices
        // 
        itemDevices.BackColor = Color.FromArgb(243, 243, 243);
        itemDevices.Dock = DockStyle.Top;
        itemDevices.Location = new Point(0, 0);
        itemDevices.Name = "itemDevices";
        itemDevices.Size = new Size(240, 40);
        itemDevices.TabIndex = 0;
        // 
        // pnlContent
        // 
        pnlContent.BackColor = Color.FromArgb(249, 249, 249);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(240, 0);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(24, 12, 24, 24);
        pnlContent.Size = new Size(860, 694);
        pnlContent.TabIndex = 0;
        // 
        // lblGreeting
        // 
        lblGreeting.Dock = DockStyle.Top;
        lblGreeting.Font = new Font("Segoe UI", 9.5F);
        lblGreeting.ForeColor = Color.FromArgb(64, 71, 82);
        lblGreeting.Location = new Point(0, 0);
        lblGreeting.Name = "lblGreeting";
        lblGreeting.Size = new Size(100, 32);
        lblGreeting.TabIndex = 0;
        lblGreeting.TextAlign = ContentAlignment.MiddleRight;
        // 
        // networkStatusBar
        // 
        networkStatusBar.BackColor = Color.FromArgb(243, 243, 243);
        networkStatusBar.Dock = DockStyle.Bottom;
        networkStatusBar.Font = new Font("Segoe UI", 9F);
        networkStatusBar.ForeColor = Color.FromArgb(26, 28, 28);
        networkStatusBar.Location = new Point(0, 694);
        networkStatusBar.Name = "networkStatusBar";
        networkStatusBar.Size = new Size(1100, 26);
        networkStatusBar.TabIndex = 2;
        // 
        // itemSupport
        // 
        //itemSupport.BackColor = Color.FromArgb(243, 243, 243);
        //itemSupport.Dock = DockStyle.Top;
        //itemSupport.Location = new Point(0, 40);
        //itemSupport.Name = "itemSupport";
        //itemSupport.Size = new Size(240, 40);
        //itemSupport.TabIndex = 2;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(1100, 720);
        Controls.Add(pnlContent);
        Controls.Add(pnlSidebar);
        Controls.Add(networkStatusBar);
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Dog Explorer";
        pnlSidebar.ResumeLayout(false);
        pnlSidebarHeader.ResumeLayout(false);
        pnlSidebarBottom.ResumeLayout(false);
        ResumeLayout(false);
    }

    private SidebarItem itemSupport;
}
