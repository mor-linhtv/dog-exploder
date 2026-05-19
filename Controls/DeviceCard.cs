using System.Drawing.Drawing2D;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class DeviceCard : UserControl
{
    public DeviceInfo Device { get; private set; } = new();

    public bool IsSelected => rbSelect.Checked;

    public event EventHandler<DeviceInfo>? Updated;

    public DeviceCard()
    {
        InitializeComponent();
    }

    public void Bind(DeviceInfo device)
    {
        Device = device;
        lblName.Text = device.Name;
        lblCategory.Text = device.Category;
        lblDetail.Text = device.Detail;
        lblChecked.Text = $"Last checked: {device.CheckedAt:HH:mm:ss}";
        lblBadge.Text = device.Status.ToString();
        Invalidate();
        lblBadge.Invalidate();
    }

    public void ClearSelection() => rbSelect.Checked = false;

    private async void BtnCheck_Click(object? sender, EventArgs e)
    {
        btnCheck.Enabled = false;
        try
        {
            var fresh = await DeviceCheckService.RecheckAsync(Device);
            Bind(fresh);
            Updated?.Invoke(this, fresh);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không check được: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCheck.Enabled = true;
        }
    }

    private void LblBadge_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var (bg, fg) = Device.Status switch
        {
            DeviceStatus.Connected    => (Color.FromArgb(0xCF, 0xE2, 0xFF), Color.FromArgb(0x00, 0x3F, 0x6D)),
            DeviceStatus.Degraded     => (Color.FromArgb(0xFD, 0xE7, 0xD3), Color.FromArgb(0x97, 0x47, 0x00)),
            DeviceStatus.Disconnected => (Color.FromArgb(0xFF, 0xDA, 0xD6), Color.FromArgb(0x93, 0x00, 0x0A)),
            _                          => (Color.FromArgb(0xE8, 0xE8, 0xE8), Color.FromArgb(0x40, 0x47, 0x52)),
        };
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, lblBadge.Width - 1, lblBadge.Height - 1), 4);
        using var bgBrush = new SolidBrush(bg);
        g.FillPath(bgBrush, path);
        TextRenderer.DrawText(g, lblBadge.Text, lblBadge.Font, new Rectangle(0, 0, lblBadge.Width, lblBadge.Height), fg,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(0xD1, 0xD1, 0xD1));
        using var path = UI.Theme.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 8);
        g.DrawPath(pen, path);
    }
}
