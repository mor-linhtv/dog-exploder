using Dog_Exploder.Controls;

namespace Dog_Exploder.Forms;

public partial class MainForm : Form
{
    private readonly Dictionary<string, UserControl> _panes = new();
    private SidebarItem[] _items => new[] { itemAllBreeds, itemFavorites, itemHistory, itemComparison, itemDevices, itemSettings, itemSupport };

    public MainForm()
    {
        InitializeComponent();
        pnlContent.Controls.Add(lblGreeting);
        Load += MainForm_Load;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        lblGreeting.Text = $"Hi, {Session.Username} 👋";
        ShowPane("breeds");
    }

    public void ShowPane(string key)
    {
        if (!_panes.TryGetValue(key, out var ctl))
        {
            ctl = CreatePane(key);
            _panes[key] = ctl;
        }
        ctl.Dock = DockStyle.Fill;

        var toRemove = pnlContent.Controls.OfType<Control>().Where(c => c != lblGreeting).ToList();
        foreach (var c in toRemove) pnlContent.Controls.Remove(c);
        pnlContent.Controls.Add(ctl);
        // Greeting must dock FIRST (it's Dock=Top reserving 32px) so the Fill pane gets the
        // remaining area. WinForms processes docking from backmost child to frontmost — keep
        // greeting at the back (higher Controls index) and push ctl to the front.
        ctl.BringToFront();
        lblGreeting.SendToBack();

        foreach (var it in _items)
            it.Active = it.Key == key || (key == "detail" && it.Key == "breeds");
    }

    public void ShowBreedDetail(Models.Breed breed)
    {
        if (!_panes.TryGetValue("detail", out var ctl))
        {
            ctl = new BreedDetailControl();
            ((BreedDetailControl)ctl).BackRequested += (s, e) => ShowPane("breeds");
            _panes["detail"] = ctl;
        }
        ((BreedDetailControl)ctl).SetBreed(breed);
        ShowPane("detail");
    }

    private UserControl CreatePane(string key)
    {
        switch (key)
        {
            case "breeds":
                var list = new BreedListControl();
                list.BreedSelected += (s, b) => ShowBreedDetail(b);
                return list;
            case "devices":
                return new DeviceStatusControl();
            default:
                var stub = new ComingSoonControl();
                stub.SetTitle($"{TitleFor(key)} — đang được phát triển");
                return stub;
        }
    }

    private static string TitleFor(string key) => key switch
    {
        "favorites"  => "Favorites",
        "history"    => "History",
        "comparison" => "Comparison",
        "settings"   => "Settings",
        "support"    => "Support",
        _ => key
    };
}
