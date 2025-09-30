namespace MIXERX.Core.Models;

public class Track
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public double? Bpm { get; set; }
    public string? Key { get; set; }
    public double? ReplayGain { get; set; }
    public DateTime LastModified { get; set; }
    public byte[]? WaveformData { get; set; }
}

public class Crate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Track> Tracks { get; set; } = new();
    public bool IsSmartCrate { get; set; }
    public string? SmartCrateQuery { get; set; }
}
