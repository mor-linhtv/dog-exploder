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
        _spinner = new LoadingSpinner();
        lblState = new Label();
        btnRetry = new Button();
        searchDebounce = new System.Windows.Forms.Timer(components);
        pnlTop.SuspendLayout();
        pnlState.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(lblTitle);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(cboGroup);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(880, 60);
        pnlTop.TabIndex = 2;
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Location = new Point(0, 10);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(300, 36);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Dog Breed Index";
        // 
        // txtSearch
        // 
        txtSearch.Font = new Font("Segoe UI", 9.5F);
        txtSearch.Location = new Point(305, 18);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Search breeds…";
        txtSearch.Size = new Size(200, 24);
        txtSearch.TabIndex = 1;
        txtSearch.TextChanged += TxtSearch_TextChanged;
        // 
        // cboGroup
        // 
        cboGroup.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGroup.Font = new Font("Segoe UI", 9.5F);
        cboGroup.Location = new Point(527, 18);
        cboGroup.Name = "cboGroup";
        cboGroup.Size = new Size(150, 25);
        cboGroup.TabIndex = 2;
        cboGroup.SelectedIndexChanged += CboGroup_SelectedIndexChanged;
        // 
        // btnRefresh
        // 
        btnRefresh.BackColor = Color.White;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(192, 199, 212);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.Font = new Font("Segoe UI", 9.5F);
        btnRefresh.Location = new Point(696, 17);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(100, 28);
        btnRefresh.TabIndex = 3;
        btnRefresh.Text = "↻ Refresh";
        btnRefresh.UseVisualStyleBackColor = false;
        btnRefresh.Click += BtnRefresh_Click;
        // 
        // pnlGrid
        // 
        pnlGrid.AutoScroll = true;
        pnlGrid.BackColor = Color.White;
        pnlGrid.Dock = DockStyle.Fill;
        pnlGrid.Location = new Point(0, 60);
        pnlGrid.Name = "pnlGrid";
        pnlGrid.Padding = new Padding(8);
        pnlGrid.Size = new Size(880, 90);
        pnlGrid.TabIndex = 0;
        // 
        // pnlState
        // 
        pnlState.BackColor = Color.White;
        pnlState.Controls.Add(_spinner);
        pnlState.Controls.Add(lblState);
        pnlState.Controls.Add(btnRetry);
        pnlState.Dock = DockStyle.Fill;
        pnlState.Location = new Point(0, 60);
        pnlState.Name = "pnlState";
        pnlState.Size = new Size(880, 90);
        pnlState.TabIndex = 1;
        pnlState.Visible = false;
        pnlState.Resize += PnlState_Resize;
        // 
        // _spinner
        // 
        _spinner.BackColor = Color.White;
        _spinner.Location = new Point(0, 0);
        _spinner.Name = "_spinner";
        _spinner.Size = new Size(40, 40);
        _spinner.TabIndex = 0;
        _spinner.Visible = false;
        // 
        // lblState
        // 
        lblState.Font = new Font("Segoe UI", 11F);
        lblState.Location = new Point(0, 0);
        lblState.Name = "lblState";
        lblState.Size = new Size(400, 30);
        lblState.TabIndex = 1;
        lblState.Text = "Đang tải...";
        lblState.TextAlign = ContentAlignment.TopCenter;
        // 
        // btnRetry
        // 
        btnRetry.BackColor = Color.FromArgb(0, 120, 212);
        btnRetry.FlatAppearance.BorderSize = 0;
        btnRetry.FlatStyle = FlatStyle.Flat;
        btnRetry.ForeColor = Color.White;
        btnRetry.Location = new Point(0, 0);
        btnRetry.Name = "btnRetry";
        btnRetry.Size = new Size(120, 32);
        btnRetry.TabIndex = 2;
        btnRetry.Text = "Thử lại";
        btnRetry.UseVisualStyleBackColor = false;
        btnRetry.Visible = false;
        btnRetry.Click += BtnRetry_Click;
        // 
        // searchDebounce
        // 
        searchDebounce.Interval = 300;
        searchDebounce.Tick += SearchDebounce_Tick;
        // 
        // BreedListControl
        // 
        BackColor = Color.FromArgb(249, 249, 249);
        Controls.Add(pnlGrid);
        Controls.Add(pnlState);
        Controls.Add(pnlTop);
        Name = "BreedListControl";
        Size = new Size(880, 150);
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlState.ResumeLayout(false);
        ResumeLayout(false);
    }
}
