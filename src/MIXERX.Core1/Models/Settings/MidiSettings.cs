namespace MIXERX.Core.Models.Settings;

public class MidiSettings
{
    public string Device { get; set; } = "none";
    public string MappingFile { get; set; } = "";
    public bool LearnMode { get; set; } = false;
    public bool ClockSync { get; set; } = false;
}