using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedDetailControl : UserControl
{
    private CancellationTokenSource? _imageCts;

    public event EventHandler? BackRequested;

    public BreedDetailControl()
    {
        InitializeComponent();
    }

    public void SetBreed(Breed breed)
    {
        lblName.Text = breed.Name;
        lblDescription.Text = breed.Description;
        lblGroupBadge.Text = string.IsNullOrEmpty(breed.GroupName) ? "—" : breed.GroupName;

        dgvSpecs.Rows.Clear();
        dgvSpecs.Rows.Add("Life Expectancy", $"{breed.Life} years");
        dgvSpecs.Rows.Add("Weight (Male)", $"{breed.MaleWeight} lb");
        dgvSpecs.Rows.Add("Weight (Female)", $"{breed.FemaleWeight} lb");
        dgvSpecs.Rows.Add("Height (Male)", $"{breed.MaleHeight} in");
        dgvSpecs.Rows.Add("Height (Female)", $"{breed.FemaleHeight} in");
        dgvSpecs.Rows.Add("Hypoallergenic", breed.Hypoallergenic ? "Yes" : "No");

        picImage.Image?.Dispose();
        picImage.Image = null;
        _imageCts?.Cancel();
        _imageCts = new CancellationTokenSource();
        _imageSpinner.Start();
        _ = LoadImageAsync(breed.Name, _imageCts.Token);
    }

    private async Task LoadImageAsync(string name, CancellationToken ct)
    {
        try
        {
            var img = await DogImageService.GetImageAsync(name, ct);
            if (ct.IsCancellationRequested || IsDisposed) return;
            BeginInvoke(() =>
            {
                if (IsDisposed) return;
                if (img != null) picImage.Image = img;
                _imageSpinner.Stop();
            });
        }
        catch
        {
            if (!IsDisposed) BeginInvoke(() => { if (!IsDisposed) _imageSpinner.Stop(); });
        }
    }

    private void Bordered_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Control c) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, c.Width - 1, c.Height - 1), 8);
        g.DrawPath(pen, path);
    }

    private void LblGroupBadge_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, lblGroupBadge.Width - 1, lblGroupBadge.Height - 1), 4);
        using var bg = new SolidBrush(Color.FromArgb(0xD3, 0xE3, 0xFF));
        g.FillPath(bg, path);
    }
}
