namespace Dog_Exploder.Models;

public class Group
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public override string ToString() => Name;
}
