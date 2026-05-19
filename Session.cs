using Dog_Exploder.Models;

namespace Dog_Exploder;

internal static class Session
{
    public static string Username { get; set; } = string.Empty;
    public static List<Breed>? Breeds { get; set; }
    public static List<Group>? Groups { get; set; }
    public static bool IsLoggingOut { get; set; }

    public static void Clear()
    {
        Username = string.Empty;
        Breeds = null;
        Groups = null;
        IsLoggingOut = false;
    }
}
