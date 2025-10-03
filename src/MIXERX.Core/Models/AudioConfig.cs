namespace MIXERX.Core.Models;

public class AudioConfig
{
    public int SampleRate { get; set; } = 48000;
    public int BufferSize { get; set; } = 128;
    public AudioApi PreferredApi { get; set; } = AudioApi.Default;
}