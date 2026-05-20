using Dog_Exploder.Models;
using Dog_Exploder.Services;

namespace Dog_Exploder.Controls;

public partial class BreedListControl : UserControl
{
    public event EventHandler<Breed>? BreedSelected;

    public BreedListControl()
    {
        InitializeComponent();
        Load += async (s, e) => await EnsureLoadedAsync();
    }

    private async Task EnsureLoadedAsync()
    {
        if (Session.Breeds == null)
            await LoadAsync();
        else
        {
            PopulateGroups();
            await RenderCardsAsync();
        }
    }

    private async Task LoadAsync()
    {
        ShowState("Đang tải dữ liệu...", showRetry: false);
        try
        {
            var breeds = await DogApiService.GetAllBreedsAsync();
            var groups = await DogApiService.GetGroupsAsync();
            DogApiService.ResolveGroupNames(breeds, groups);
            Session.Breeds = breeds;
            Session.Groups = groups;
            PopulateGroups();
            await RenderCardsAsync();
            HideState();
        }
        catch (Exception ex)
        {
            ShowState($"Không tải được dữ liệu: {ex.Message}", showRetry: true);
        }
    }

    private void PopulateGroups()
    {
        cboGroup.Items.Clear();
        cboGroup.Items.Add("All Groups");
        if (Session.Groups != null)
            foreach (var g in Session.Groups.OrderBy(x => x.Name))
                cboGroup.Items.Add(g);
        cboGroup.SelectedIndex = 0;
    }

    private async Task RenderCardsAsync()
    {
        if (Session.Breeds == null) return;
        var query = txtSearch.Text.Trim();
        var group = cboGroup.SelectedItem as Group;

        var filtered = Session.Breeds
            .Where(b => string.IsNullOrEmpty(query) ||
                        b.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Where(b => group == null || b.GroupId == group.Id)
            .Take(200)
            .ToList();

        pnlGrid.SuspendLayout();
        foreach (Control c in pnlGrid.Controls) c.Dispose();
        pnlGrid.Controls.Clear();
        for (int i = 0; i < filtered.Count; i++)
        {
            var card = new BreedCard();
            card.Bind(filtered[i]);
            card.BreedSelected += (s, breed) => BreedSelected?.Invoke(this, breed);
            pnlGrid.Controls.Add(card);
            if ((i + 1) % 20 == 0)
            {
                pnlGrid.ResumeLayout(false);
                await Task.Yield();
                pnlGrid.SuspendLayout();
            }
        }
        pnlGrid.ResumeLayout();
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        searchDebounce.Stop();
        searchDebounce.Start();
    }

    private async void CboGroup_SelectedIndexChanged(object? sender, EventArgs e) => await RenderCardsAsync();

    private async void BtnRefresh_Click(object? sender, EventArgs e)
    {
        Session.Breeds = null;
        Session.Groups = null;
        DogImageService.ClearCache();
        await LoadAsync();
    }

    private async void BtnRetry_Click(object? sender, EventArgs e) => await LoadAsync();

    private void ShowState(string text, bool showRetry)
    {
        lblState.Text = text;
        btnRetry.Visible = showRetry;
        if (showRetry) _spinner.Stop(); else _spinner.Start();
        pnlState.Visible = true;
        pnlState.BringToFront();
    }

    private void HideState()
    {
        _spinner.Stop();
        pnlState.Visible = false;
        pnlGrid.BringToFront();
    }
}
