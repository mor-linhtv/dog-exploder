using System.Diagnostics;
using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class DeviceStatusControl : UserControl
{
    public DeviceStatusControl()
    {
        InitializeComponent();
        Load += async (s, e) => await RefreshAllAsync();
    }

    private async Task RefreshAllAsync()
    {
        ShowState("Đang kiểm tra thiết bị...");
        try
        {
            var devices = await DeviceCheckService.EnumerateAsync();
            HideState();
            RenderCards(devices);
            lblLastUpdated.Text = $"Last updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            ShowState($"Lỗi: {ex.Message}");
        }
    }

    private void RenderCards(List<DeviceInfo> devices)
    {
        pnlDevices.SuspendLayout();
        foreach (Control c in pnlDevices.Controls) c.Dispose();
        pnlDevices.Controls.Clear();
        foreach (var d in devices)
        {
            var card = new DeviceCard();
            card.Bind(d);
            pnlDevices.Controls.Add(card);
        }
        pnlDevices.ResumeLayout();
    }

    private async void BtnRefreshAll_Click(object? sender, EventArgs e) => await RefreshAllAsync();

    private async void BtnExportSelected_Click(object? sender, EventArgs e)
    {
        var selected = pnlDevices.Controls.OfType<DeviceCard>().FirstOrDefault(c => c.IsSelected);
        if (selected == null)
        {
            MessageBox.Show("Vui lòng chọn 1 thiết bị để export.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "Excel files|*.xlsx",
            FileName = "device-status-log.xlsx",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            OverwritePrompt = false
        };
        if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;

        try
        {
            var path = dlg.FileName;
            var device = selected.Device;
            device.CheckedAt = DateTime.Now;
            await Task.Run(() => ExcelExportService.AppendDeviceStatus(path, device));

            var open = MessageBox.Show($"Đã ghi vào:\n{path}\n\nMở thư mục chứa?", "Thành công",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (open == DialogResult.Yes)
                Process.Start("explorer.exe", $"/select,\"{path}\"");
        }
        catch (IOException)
        {
            MessageBox.Show("File đang được mở. Vui lòng đóng Excel rồi thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi ghi file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowState(string text)
    {
        lblState.Text = text;
        pnlState.Visible = true;
        pnlState.BringToFront();
    }

    private void HideState()
    {
        pnlState.Visible = false;
        pnlDevices.BringToFront();
    }
}
