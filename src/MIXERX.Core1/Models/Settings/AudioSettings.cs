namespace MIXERX.Core.Models.Settings;

public class AudioSettings
{
    public string Driver { get; set; } = "WASAPI";
    public int SampleRate { get; set; } = 48000;
    public int BufferSize { get; set; } = 512;
    public string OutputDevice { get; set; } = "default";
    public float MasterVolume { get; set; } = 0.85f;
    public float CueVolume { get; set; } = 0.75f;
}