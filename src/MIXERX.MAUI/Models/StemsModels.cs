namespace MIXERX.MAUI.Models;

public enum StemType
{
    Vocals,
    Melody,
    Bass,
    Drums
}

public class StemControl
{
    public StemType Type { get; set; }
    public float Volume { get; set; } = 1.0f;
    public bool IsMuted { get; set; }
    public bool IsSolo { get; set; }
}
