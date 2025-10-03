namespace MIXERX.Core.Models.Settings;

public class DeckSettings
{
    public int TempoRange { get; set; } = 8;
    public bool KeylockDefault { get; set; } = false;
    public string SyncMode { get; set; } = "manual";
    public float WaveformZoom { get; set; } = 1.0f;
    public string WaveformColors { get; set; } = "default";
    public bool EnergyOverlay { get; set; } = true;
    public bool BeatMarkers { get; set; } = true;
}