using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class DeckViewModel : ObservableObject
{
    [ObservableProperty]
    private string deckName = "DECK";

    [ObservableProperty]
    private Color deckColor = Colors.Cyan;

    [ObservableProperty]
    private string trackName = "No Track Loaded";

    [ObservableProperty]
    private string bpm = "120.0 BPM";

    [ObservableProperty]
    private string key = "Am";

    [ObservableProperty]
    private bool isPlaying = false;

    [ObservableProperty]
    private double position = 0.0;

    [ObservableProperty]
    private double tempo = 0.0;

    [ObservableProperty]
    private float[] waveformData = Array.Empty<float>();

    public string PlayButtonText => IsPlaying ? "⏸" : "▶";
    public string TempoDisplay => $"{(Tempo >= 0 ? "+" : "")}{Tempo:F1}%";

    [RelayCommand]
    private void PlayPause()
    {
        IsPlaying = !IsPlaying;
        OnPropertyChanged(nameof(PlayButtonText));
    }

    [RelayCommand]
    private void SetCue(object parameter)
    {
        // TODO: Set hot cue
    }

    [RelayCommand]
    private void AutoLoop(object parameter)
    {
        // TODO: Set auto loop
    }

    [RelayCommand]
    private void ExitLoop()
    {
        // TODO: Exit loop
    }

    [RelayCommand]
    private void Sync()
    {
        // TODO: Sync to master
    }
}
