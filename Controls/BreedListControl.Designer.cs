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
    private LoadingSpinner _spinner;
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

        _spinner = new LoadingSpinner();
        _spinner.BackColor = Color.White;

        lblState.Text = "Đang tải...";
        lblState.Font = new Font("Segoe UI", 11f);
        lblState.TextAlign = ContentAlignment.TopCenter;
        lblState.Size = new Size(400, 30);

        btnRetry.Text = "Thử lại";
        btnRetry.Visible = false;
        btnRetry.Size = new Size(120, 32);
        btnRetry.FlatStyle = FlatStyle.Flat;
        btnRetry.BackColor = Color.FromArgb(0x00, 0x78, 0xD4);
        btnRetry.ForeColor = Color.White;
        btnRetry.FlatAppearance.BorderSize = 0;
        btnRetry.Click += BtnRetry_Click;

        pnlState.Controls.Add(_spinner);
        pnlState.Controls.Add(lblState);
        pnlState.Controls.Add(btnRetry);
        pnlState.Resize += (s, e) =>
        {
            _spinner.Location = new Point((pnlState.Width - 40) / 2, pnlState.Height / 2 - 60);
            lblState.Location = new Point((pnlState.Width - 400) / 2, pnlState.Height / 2 - 10);
            btnRetry.Location = new Point((pnlState.Width - btnRetry.Width) / 2, pnlState.Height / 2 + 30);
        };

        searchDebounce.Interval = 300;
        searchDebounce.Tick += async (s, e) => { searchDebounce.Stop(); await RenderCardsAsync(); };

        Controls.Add(pnlGrid);
        Controls.Add(pnlState);
        Controls.Add(pnlTop);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
