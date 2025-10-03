namespace MIXERX.Core.Models;

public class SampleSlot
{
    public string? FilePath { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsLoaded => !string.IsNullOrEmpty(FilePath);
    public bool IsPlaying { get; set; }
    public PlayMode PlayMode { get; set; } = PlayMode.Trigger;
    public bool SyncEnabled { get; set; }
    public float Volume { get; set; } = 1.0f;
}
