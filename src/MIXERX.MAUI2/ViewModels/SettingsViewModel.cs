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

    // Library Settings
    [ObservableProperty]
    private bool showItunesLibrary = true;

    [ObservableProperty]
    private bool protectLibrary = false;

    [ObservableProperty]
    private bool customCrateColumns = false;

    [ObservableProperty]
    private bool centerOnSelectedSong = false;

    [ObservableProperty]
    private bool includeSubcrateTracks = true;

    [ObservableProperty]
    private string playedTrackColor = "Blue";

    [ObservableProperty]
    private bool resetPlayedTracksOnExit = false;

    [ObservableProperty]
    private double libraryTextSize = 12;

    // Display Settings
    [ObservableProperty]
    private bool showTempoMatchingDisplay = false;

    [ObservableProperty]
    private bool hideTrackArtist = false;

    [ObservableProperty]
    private bool eqColoredWaveforms = false;

    [ObservableProperty]
    private bool colorKeyDisplay = true;

    [ObservableProperty]
    private bool performancePadCueLayout = true;

    [ObservableProperty]
    private string showKeyAs = "Camelot";

    [ObservableProperty]
    private bool hiResScreenDisplay = false;

    [ObservableProperty]
    private bool deckBpmDisplay1Decimal = true;

    [ObservableProperty]
    private bool deckBpmDisplay2Decimal = false;

    [ObservableProperty]
    private double maxScreenUpdates = 30;

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
    private bool sendAnalytics = true;

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
        ShowItunesLibrary = true;
        ProtectLibrary = false;
        ColorKeyDisplay = true;
        SendAnalytics = true;
    }
}
