namespace MIXERX.Core.Models.Settings;

public class PerformanceSettings
{
    public int HotCueCount { get; set; } = 4;
    public List<string> HotCueColors { get; set; } = new() { "#ff0044", "#ff8800", "#00ff88", "#0088ff" };
    public string PadLayout { get; set; } = "default";
    public bool FullScreenDefault { get; set; } = false;
}