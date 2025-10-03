namespace MIXERX.Core.Models.Settings;

public class LoopSettings
{
    public int DefaultLength { get; set; } = 4;
    public bool Quantize { get; set; } = true;
    public bool RollEnabled { get; set; } = false;
}