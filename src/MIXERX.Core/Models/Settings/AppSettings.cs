namespace MIXERX.Core.Models.Settings;

public class AppSettings
{
    public AudioSettings Audio { get; set; } = new();
    public DeckSettings Decks { get; set; } = new();
    public EffectsSettings Effects { get; set; } = new();
    public LoopSettings Loops { get; set; } = new();
    public RecordingSettings Recording { get; set; } = new();
    public LibrarySettings Library { get; set; } = new();
    public DisplaySettings Display { get; set; } = new();
    public MidiSettings Midi { get; set; } = new();
    public PerformanceSettings Performance { get; set; } = new();
    public PrivacySettings Privacy { get; set; } = new();
}