namespace MIXERX.Core.Models;

public class Crate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Track> Tracks { get; set; } = new();
    public bool IsSmartCrate { get; set; }
    public string? SmartCrateQuery { get; set; }
}