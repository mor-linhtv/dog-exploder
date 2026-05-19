using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedCard : UserControl
{
    private readonly CancellationTokenSource _cts = new();
    private Breed _breed = null!;

    public event EventHandler<Breed>? BreedSelected;

    public BreedCard()
    {
        InitializeComponent();
    }

    public void Bind(Breed breed)
    {
        _breed = breed;
        lblName.Text = breed.Name;
        lblBadge.Text = "  " + (breed.GroupName ?? "—") + (breed.Hypoallergenic ? "  • Hypoallergenic" : "");
        lblDesc.Text = breed.Description;
        _ = LoadImageAsync();
    }

    private async Task LoadImageAsync()
    {
        try
        {
            var img = await DogImageService.GetImageAsync(_breed.Name, _cts.Token);
            if (IsDisposed || _cts.IsCancellationRequested) return;
            if (img != null && !IsDisposed)
                BeginInvoke(() => { if (!IsDisposed) picImage.Image = img; });
        }
        catch { }
    }

    private void PicImage_Paint(object? sender, PaintEventArgs e)
    {
        if (picImage.Image != null) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(Color.FromArgb(0xC0, 0xC7, 0xD4));
        var cx = picImage.Width / 2; var cy = picImage.Height / 2;
        g.FillEllipse(brush, cx - 16, cy - 4, 8, 8);
        g.FillEllipse(brush, cx - 6, cy - 10, 8, 8);
        g.FillEllipse(brush, cx + 4, cy - 10, 8, 8);
        g.FillEllipse(brush, cx + 14, cy - 4, 8, 8);
        g.FillEllipse(brush, cx - 9, cy + 4, 18, 12);
    }

    private void LblBadge_Paint(object? sender, PaintEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(lblBadge.Text)) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var size = TextRenderer.MeasureText(lblBadge.Text, lblBadge.Font);
        var rect = new Rectangle(0, 0, Math.Min(size.Width + 12, lblBadge.Width), lblBadge.Height - 2);
        using var path = UI.Theme.RoundedRect(rect, 4);
        using var bg = new SolidBrush(Color.FromArgb(0xD3, 0xE3, 0xFF));
        g.FillPath(bg, path);
        TextRenderer.DrawText(g, lblBadge.Text, lblBadge.Font, rect, lblBadge.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = UI.Theme.RoundedRect(r, 8);
        g.DrawPath(pen, path);
    }

    private void Card_Click(object? sender, EventArgs e) => BreedSelected?.Invoke(this, _breed);
}
