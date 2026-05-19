using Dog_Exploder.Forms;

namespace Dog_Exploder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) return;
            Session.Username = login.Username;
            Application.Run(new MainForm());
        }
    }
}
