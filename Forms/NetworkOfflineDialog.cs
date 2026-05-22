namespace Dog_Exploder.Forms;

public partial class NetworkOfflineDialog : Form
{
    public NetworkOfflineDialog()
    {
        InitializeComponent();
    }

    public void SetDetectedAt(DateTime t)
    {
        lblDetectedAt.Text = $"Phát hiện lúc {t:HH:mm:ss}";
    }
}
