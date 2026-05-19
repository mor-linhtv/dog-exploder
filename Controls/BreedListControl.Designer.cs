namespace Dog_Exploder.Controls;

partial class BreedListControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTop;
    private Label lblTitle;
    private TextBox txtSearch;
    private ComboBox cboGroup;
    private Button btnRefresh;
    private FlowLayoutPanel pnlGrid;
    private Panel pnlState;
    private Label lblState;
    private Button btnRetry;
    private System.Windows.Forms.Timer searchDebounce;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTop = new Panel();
        lblTitle = new Label();
        txtSearch = new TextBox();
        cboGroup = new ComboBox();
        btnRefresh = new Button();
        pnlGrid = new FlowLayoutPanel();
        pnlState = new Panel();
        lblState = new Label();
        btnRetry = new Button();
        searchDebounce = new System.Windows.Forms.Timer(components);

        pnlTop.Dock = DockStyle.Top;
        pnlTop.Height = 60;

        lblTitle.Text = "Dog Breed Index";
        lblTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
        lblTitle.Location = new Point(0, 10);
        lblTitle.Size = new Size(300, 36);

        txtSearch.PlaceholderText = "Search breeds…";
        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Location = new Point(320, 18);
        txtSearch.Size = new Size(240, 26);
        txtSearch.TextChanged += TxtSearch_TextChanged;

        cboGroup.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGroup.Font = new Font("Segoe UI", 9.5f);
        cboGroup.Location = new Point(570, 18);
        cboGroup.Size = new Size(180, 26);
        cboGroup.SelectedIndexChanged += CboGroup_SelectedIndexChanged;

        btnRefresh.Text = "↻ Refresh";
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnRefresh.BackColor = Color.White;
        btnRefresh.Location = new Point(760, 17);
        btnRefresh.Size = new Size(100, 28);
        btnRefresh.Click += BtnRefresh_Click;

        pnlTop.Controls.Add(lblTitle);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(cboGroup);
        pnlTop.Controls.Add(btnRefresh);

        pnlGrid.Dock = DockStyle.Fill;
        pnlGrid.AutoScroll = true;
        pnlGrid.BackColor = Color.White;
        pnlGrid.FlowDirection = FlowDirection.LeftToRight;
        pnlGrid.WrapContents = true;
        pnlGrid.Padding = new Padding(8);

        pnlState.Dock = DockStyle.Fill;
        pnlState.BackColor = Color.White;
        pnlState.Visible = false;

        lblState.Text = "Đang tải...";
        lblState.Font = new Font("Segoe UI", 11f);
        lblState.Dock = DockStyle.Top;
        lblState.Height = 40;
        lblState.TextAlign = ContentAlignment.MiddleCenter;
        lblState.Padding = new Padding(0, 100, 0, 0);

        btnRetry.Text = "Thử lại";
        btnRetry.Visible = false;
        btnRetry.Size = new Size(120, 32);
        btnRetry.Anchor = AnchorStyles.Top;
        btnRetry.Top = 160;
        btnRetry.Left = 0;
        btnRetry.FlatStyle = FlatStyle.Flat;
        btnRetry.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnRetry.ForeColor = Color.White;
        btnRetry.FlatAppearance.BorderSize = 0;
        btnRetry.Click += BtnRetry_Click;

        pnlState.Controls.Add(btnRetry);
        pnlState.Controls.Add(lblState);
        pnlState.Resize += (s, e) => { btnRetry.Left = (pnlState.Width - btnRetry.Width) / 2; };

        searchDebounce.Interval = 300;
        searchDebounce.Tick += (s, e) => { searchDebounce.Stop(); RenderCards(); };

        Controls.Add(pnlGrid);
        Controls.Add(pnlState);
        Controls.Add(pnlTop);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
