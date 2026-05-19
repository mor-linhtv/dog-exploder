namespace Dog_Exploder.Controls;

partial class DeviceStatusControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTop;
    private Button btnRefreshAll;
    private Button btnAddDevice;
    private Label lblLastUpdated;
    private Panel pnlBottom;
    private Button btnExportSelected;
    private FlowLayoutPanel pnlDevices;
    private Panel pnlState;
    private Label lblState;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTop = new Panel();
        btnRefreshAll = new Button();
        btnAddDevice = new Button();
        lblLastUpdated = new Label();
        pnlBottom = new Panel();
        btnExportSelected = new Button();
        pnlDevices = new FlowLayoutPanel();
        pnlState = new Panel();
        lblState = new Label();

        pnlTop.Dock = DockStyle.Top;
        pnlTop.Height = 60;

        btnRefreshAll.Text = "↻ Refresh All";
        btnRefreshAll.Location = new Point(0, 14);
        btnRefreshAll.Size = new Size(130, 32);
        btnRefreshAll.FlatStyle = FlatStyle.Flat;
        btnRefreshAll.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnRefreshAll.ForeColor = Color.White;
        btnRefreshAll.FlatAppearance.BorderSize = 0;
        btnRefreshAll.Click += BtnRefreshAll_Click;

        btnAddDevice.Text = "+ Add Device";
        btnAddDevice.Location = new Point(140, 14);
        btnAddDevice.Size = new Size(130, 32);
        btnAddDevice.FlatStyle = FlatStyle.Flat;
        btnAddDevice.BackColor = Color.White;
        btnAddDevice.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnAddDevice.Enabled = false;

        lblLastUpdated.Text = "Last updated: —";
        lblLastUpdated.Font = new Font("Segoe UI", 9f);
        lblLastUpdated.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblLastUpdated.Location = new Point(280, 22);
        lblLastUpdated.Size = new Size(500, 22);

        pnlTop.Controls.Add(btnRefreshAll);
        pnlTop.Controls.Add(btnAddDevice);
        pnlTop.Controls.Add(lblLastUpdated);
        pnlTop.Resize += (s, e) => { lblLastUpdated.Left = pnlTop.Width - lblLastUpdated.Width - 8; };

        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Height = 60;

        btnExportSelected.Text = "Export trạng thái thiết bị (.xlsx)";
        btnExportSelected.Size = new Size(280, 36);
        btnExportSelected.Location = new Point(0, 14);
        btnExportSelected.FlatStyle = FlatStyle.Flat;
        btnExportSelected.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnExportSelected.ForeColor = Color.White;
        btnExportSelected.FlatAppearance.BorderSize = 0;
        btnExportSelected.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnExportSelected.Click += BtnExportSelected_Click;

        pnlBottom.Controls.Add(btnExportSelected);
        pnlBottom.Resize += (s, e) => { btnExportSelected.Left = pnlBottom.Width - btnExportSelected.Width - 8; };

        pnlDevices.Dock = DockStyle.Fill;
        pnlDevices.AutoScroll = true;
        pnlDevices.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
        pnlDevices.FlowDirection = FlowDirection.LeftToRight;
        pnlDevices.WrapContents = true;
        pnlDevices.Padding = new Padding(8);

        pnlState.Dock = DockStyle.Fill;
        pnlState.Visible = false;
        pnlState.BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);

        lblState.Text = "Đang kiểm tra thiết bị...";
        lblState.Dock = DockStyle.Fill;
        lblState.TextAlign = ContentAlignment.MiddleCenter;
        lblState.Font = new Font("Segoe UI", 11f);
        pnlState.Controls.Add(lblState);

        Controls.Add(pnlDevices);
        Controls.Add(pnlState);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
