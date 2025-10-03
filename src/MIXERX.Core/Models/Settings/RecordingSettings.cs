namespace MIXERX.Core.Models.Settings;

public class RecordingSettings
{
    public string Format { get; set; } = "mp3";
    public int Mp3Bitrate { get; set; } = 192;
    public string Directory { get; set; } = "";
    public bool AutoDeleteWav { get; set; } = true;
    public string FileNamingPattern { get; set; } = "MIXERX_Recording_{timestamp}";
}