namespace Dog_Exploder.Controls;

public partial class ComingSoonControl : UserControl
{
    public ComingSoonControl()
    {
        InitializeComponent();
    }

    public void SetTitle(string title) => lblTitle.Text = title;
}
