namespace Dog_Exploder.Models;

public class Breed
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BreedRange Life { get; set; } = new();
    public BreedRange MaleWeight { get; set; } = new();
    public BreedRange FemaleWeight { get; set; } = new();
    public BreedRange MaleHeight { get; set; } = new();
    public BreedRange FemaleHeight { get; set; } = new();
    public bool Hypoallergenic { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
}
