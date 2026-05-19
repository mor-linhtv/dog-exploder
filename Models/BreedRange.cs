namespace Dog_Exploder.Models;

public class BreedRange
{
    public int Min { get; set; }
    public int Max { get; set; }
    public override string ToString() => Min == Max ? Min.ToString() : $"{Min} - {Max}";
}
