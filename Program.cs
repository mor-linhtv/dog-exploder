using Dog_Exploder.Forms;

namespace Dog_Exploder;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new AppApplicationContext());
    }
}

internal sealed class AppApplicationContext : ApplicationContext
{
    public AppApplicationContext() => ShowLogin();

    private void ShowLogin()
    {
        Session.Clear();
        using var login = new LoginForm();
        if (login.ShowDialog() != DialogResult.OK)
        {
            ExitThread();
            return;
        }
        Session.Username = login.Username;
        var main = new MainForm();
        main.FormClosed += OnMainFormClosed;
        MainForm = main;
        main.Show();
    }

    private void OnMainFormClosed(object? sender, FormClosedEventArgs e)
    {
        (sender as Form)?.Dispose();
        if (Session.IsLoggingOut)
            ShowLogin();
        else
            ExitThread();
    }
}
