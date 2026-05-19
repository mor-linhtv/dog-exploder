namespace Dog_Exploder.Controls;

partial class BreedDetailControl
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlTopBar;
    private Button btnBack;
    private Button btnEdit;
    private Panel pnlHeader;
    private PictureBox picImage;
    private Label lblName;
    private Label lblDescription;
    private Label lblGroupBadge;
    private TableLayoutPanel pnlBody;
    private Panel pnlSpecs;
    private Label lblSpecsTitle;
    private DataGridView dgvSpecs;
    private Panel pnlCare;
    private Label lblCareTitle;
    private Label lblCareBody;
    private LoadingSpinner _imageSpinner;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTopBar = new Panel();
        btnBack = new Button();
        btnEdit = new Button();
        pnlHeader = new Panel();
        picImage = new PictureBox();
        lblName = new Label();
        lblDescription = new Label();
        lblGroupBadge = new Label();
        pnlBody = new TableLayoutPanel();
        pnlSpecs = new Panel();
        lblSpecsTitle = new Label();
        dgvSpecs = new DataGridView();
        pnlCare = new Panel();
        lblCareTitle = new Label();
        lblCareBody = new Label();

        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 50;

        btnBack.Text = "← Back to List";
        btnBack.Location = new Point(0, 10);
        btnBack.Size = new Size(130, 30);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.FlatAppearance.BorderColor = Color.FromArgb(0xC0, 0xC7, 0xD4);
        btnBack.BackColor = Color.White;
        btnBack.Click += (s, e) => BackRequested?.Invoke(this, EventArgs.Empty);

        btnEdit.Text = "Edit Record";
        btnEdit.Location = new Point(660, 10);
        btnEdit.Size = new Size(110, 30);
        btnEdit.FlatStyle = FlatStyle.Flat;
        btnEdit.BackColor = Color.White;
        btnEdit.Enabled = false;

        pnlTopBar.Controls.Add(btnBack);
        pnlTopBar.Controls.Add(btnEdit);

        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 220;
        pnlHeader.BackColor = Color.White;
        pnlHeader.Padding = new Padding(16);
        pnlHeader.Paint += Bordered_Paint;

        picImage.Location = new Point(16, 16);
        picImage.Size = new Size(280, 180);
        picImage.SizeMode = PictureBoxSizeMode.Zoom;
        picImage.BackColor = Color.FromArgb(0xE8, 0xE8, 0xE8);

        lblName.Location = new Point(320, 16);
        lblName.Size = new Size(500, 40);
        lblName.Font = new Font("Segoe UI", 20f, FontStyle.Bold);

        lblDescription.Location = new Point(320, 60);
        lblDescription.Size = new Size(500, 100);
        lblDescription.Font = new Font("Segoe UI", 10f);
        lblDescription.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblDescription.AutoEllipsis = true;

        lblGroupBadge.Location = new Point(320, 168);
        lblGroupBadge.Size = new Size(220, 24);
        lblGroupBadge.Padding = new Padding(8, 4, 8, 4);
        lblGroupBadge.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblGroupBadge.ForeColor = Color.FromArgb(0x00, 0x48, 0x83);
        lblGroupBadge.Paint += LblGroupBadge_Paint;

        _imageSpinner = new LoadingSpinner();
        _imageSpinner.Size = new Size(40, 40);
        _imageSpinner.Location = new Point(
            picImage.Left + (picImage.Width  - 40) / 2,
            picImage.Top  + (picImage.Height - 40) / 2);
        _imageSpinner.BackColor = picImage.BackColor;

        pnlHeader.Controls.Add(picImage);
        pnlHeader.Controls.Add(_imageSpinner);
        _imageSpinner.BringToFront();
        pnlHeader.Controls.Add(lblName);
        pnlHeader.Controls.Add(lblDescription);
        pnlHeader.Controls.Add(lblGroupBadge);

        pnlSpecs.Dock = DockStyle.Fill;
        pnlSpecs.Margin = new Padding(0, 0, 8, 0);
        pnlSpecs.BackColor = Color.White;
        pnlSpecs.Padding = new Padding(16);
        pnlSpecs.Paint += Bordered_Paint;

        lblSpecsTitle.Text = "Breed Specifications";
        lblSpecsTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        lblSpecsTitle.Dock = DockStyle.Top;
        lblSpecsTitle.Height = 30;

        dgvSpecs.Dock = DockStyle.Fill;
        dgvSpecs.AllowUserToAddRows = false;
        dgvSpecs.AllowUserToDeleteRows = false;
        dgvSpecs.ReadOnly = true;
        dgvSpecs.RowHeadersVisible = false;
        dgvSpecs.ColumnHeadersVisible = false;
        dgvSpecs.BackgroundColor = Color.White;
        dgvSpecs.BorderStyle = BorderStyle.None;
        dgvSpecs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSpecs.Columns.Add("Key", "");
        dgvSpecs.Columns.Add("Val", "");
        dgvSpecs.Columns[0].Width = 160;
        dgvSpecs.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvSpecs.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
        dgvSpecs.GridColor = Color.FromArgb(0xE2, 0xE2, 0xE2);

        pnlSpecs.Controls.Add(dgvSpecs);
        pnlSpecs.Controls.Add(lblSpecsTitle);

        pnlCare.Dock = DockStyle.Fill;
        pnlCare.Margin = new Padding(8, 0, 0, 0);
        pnlCare.BackColor = Color.White;
        pnlCare.Padding = new Padding(16);
        pnlCare.Paint += Bordered_Paint;

        lblCareTitle.Text = "Temperament & Care";
        lblCareTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        lblCareTitle.Dock = DockStyle.Top;
        lblCareTitle.Height = 30;

        lblCareBody.Text = "• Trainability: N/A\n• Energy: N/A\n• Shedding: N/A\n\n(API không cung cấp thông tin này.)";
        lblCareBody.Font = new Font("Segoe UI", 9.5f);
        lblCareBody.ForeColor = Color.FromArgb(0x40, 0x47, 0x52);
        lblCareBody.Dock = DockStyle.Fill;

        pnlCare.Controls.Add(lblCareBody);
        pnlCare.Controls.Add(lblCareTitle);

        pnlBody.Dock = DockStyle.Fill;
        pnlBody.ColumnCount = 2;
        pnlBody.RowCount = 1;
        pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        pnlBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        pnlBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        pnlBody.Padding = new Padding(0, 16, 0, 0);
        pnlBody.Controls.Add(pnlSpecs, 0, 0);
        pnlBody.Controls.Add(pnlCare, 1, 0);

        Controls.Add(pnlBody);
        Controls.Add(pnlHeader);
        Controls.Add(pnlTopBar);
        BackColor = Color.FromArgb(0xF9, 0xF9, 0xF9);
    }
}
