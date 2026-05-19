using Dog_Exploder.Models;

namespace Dog_Exploder;

internal static class Session
{
    public static string Username { get; set; } = string.Empty;
    public static List<Breed>? Breeds { get; set; }
    public static List<Group>? Groups { get; set; }
}
