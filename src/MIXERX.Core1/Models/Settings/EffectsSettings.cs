namespace MIXERX.Core.Models.Settings;

public class EffectsSettings
{
    public float ReverbWet { get; set; } = 0.3f;
    public float DelayMix { get; set; } = 0.5f;
    public float FilterCutoff { get; set; } = 0.5f;
    public bool BypassOnLoad { get; set; } = false;
}