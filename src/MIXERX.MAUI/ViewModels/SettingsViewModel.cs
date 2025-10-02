using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    // Audio Settings
    [ObservableProperty]
    private string audioDriver = "WASAPI";

    [ObservableProperty]
    private int sampleRate = 48000;

    [ObservableProperty]
    private int bufferSize = 512;

    [ObservableProperty]
    private double masterVolume = 85;

    [ObservableProperty]
    private double cueVolume = 75;

    // Deck Settings
    [ObservableProperty]
    private string tempoRange = "±8%";

    [ObservableProperty]
    private bool keylockDefaultOn = false;

    [ObservableProperty]
    private bool showEnergyOverlay = true;

    [ObservableProperty]
    private bool showBeatMarkers = true;

    [ObservableProperty]
    private double waveformZoom = 1.0;

    // Effects Settings
    [ObservableProperty]
    private bool effectsEnabled = true;

    [ObservableProperty]
    private string effectsQuality = "High";

    // Recording Settings
    [ObservableProperty]
    private string recordingFormat = "WAV";

    [ObservableProperty]
    private int recordingBitrate = 320;

    [ObservableProperty]
    private string recordingPath = "";

    // Display Settings
    [ObservableProperty]
    private bool darkMode = true;

    [ObservableProperty]
    private string displayMode = "Horizontal";

    // Privacy Settings
    [ObservableProperty]
    private bool sendAnalytics = false;

    [ObservableProperty]
    private bool autoUpdate = true;

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        // TODO: Save settings to file
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void ResetSettings()
    {
        // Reset to defaults
        AudioDriver = "WASAPI";
        SampleRate = 48000;
        BufferSize = 512;
        MasterVolume = 85;
        CueVolume = 75;
        TempoRange = "±8%";
        KeylockDefaultOn = false;
        ShowEnergyOverlay = true;
        ShowBeatMarkers = true;
        WaveformZoom = 1.0;
    }
}
