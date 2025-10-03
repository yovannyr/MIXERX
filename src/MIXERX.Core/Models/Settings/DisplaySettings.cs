namespace MIXERX.Core.Models.Settings;

public class DisplaySettings
{
    public int BpmDecimals { get; set; } = 1;
    public string KeyNotation { get; set; } = "camelot";
    public bool TempoMatching { get; set; } = false;
    public bool EqWaveforms { get; set; } = false;
    public bool HiRes { get; set; } = false;
    public int MaxFps { get; set; } = 30;
    public bool ColorKeyDisplay { get; set; } = true;
}