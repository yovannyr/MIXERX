namespace MIXERX.Core.Settings;

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

public class AudioSettings
{
    public string Driver { get; set; } = "WASAPI";
    public int SampleRate { get; set; } = 48000;
    public int BufferSize { get; set; } = 512;
    public string OutputDevice { get; set; } = "default";
    public float MasterVolume { get; set; } = 0.85f;
    public float CueVolume { get; set; } = 0.75f;
}

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

public class EffectsSettings
{
    public float ReverbWet { get; set; } = 0.3f;
    public float DelayMix { get; set; } = 0.5f;
    public float FilterCutoff { get; set; } = 0.5f;
    public bool BypassOnLoad { get; set; } = false;
}

public class LoopSettings
{
    public int DefaultLength { get; set; } = 4;
    public bool Quantize { get; set; } = true;
    public bool RollEnabled { get; set; } = false;
}

public class RecordingSettings
{
    public string Format { get; set; } = "mp3";
    public int Mp3Bitrate { get; set; } = 192;
    public string Directory { get; set; } = "";
    public bool AutoDeleteWav { get; set; } = true;
    public string FileNamingPattern { get; set; } = "MIXERX_Recording_{timestamp}";
}

public class LibrarySettings
{
    public List<string> Directories { get; set; } = new();
    public bool AutoImport { get; set; } = true;
    public bool ShowItunes { get; set; } = false;
    public bool AnalyzeOnImport { get; set; } = true;
    public string PlayedTrackColor { get; set; } = "blue";
    public int TextSize { get; set; } = 12;
    public bool ProtectLibrary { get; set; } = false;
    public bool CenterOnSelected { get; set; } = false;
}

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

public class MidiSettings
{
    public string Device { get; set; } = "none";
    public string MappingFile { get; set; } = "";
    public bool LearnMode { get; set; } = false;
    public bool ClockSync { get; set; } = false;
}

public class PerformanceSettings
{
    public int HotCueCount { get; set; } = 4;
    public List<string> HotCueColors { get; set; } = new() { "#ff0044", "#ff8800", "#00ff88", "#0088ff" };
    public string PadLayout { get; set; } = "default";
    public bool FullScreenDefault { get; set; } = false;
}

public class PrivacySettings
{
    public bool SendUsageData { get; set; } = true;
    public bool CrashReports { get; set; } = true;
    public bool AnonymousStats { get; set; } = true;
}
