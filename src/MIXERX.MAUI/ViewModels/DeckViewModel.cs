using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class HotCueViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isSet;

    [ObservableProperty]
    private Color color = Colors.Red;

    [ObservableProperty]
    private string label = string.Empty;
}

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

    [ObservableProperty]
    private bool isSynced = false;

    [ObservableProperty]
    private bool isLooping = false;

    [ObservableProperty]
    private int loopLengthBeats = 4;

    [ObservableProperty]
    private float loopProgress = 0f;

    // EQ Controls
    [ObservableProperty]
    private double eqLow = 1.0;

    [ObservableProperty]
    private double eqMid = 1.0;

    [ObservableProperty]
    private double eqHigh = 1.0;

    // Hot Cues
    public HotCueViewModel HotCue1 { get; } = new();
    public HotCueViewModel HotCue2 { get; } = new();
    public HotCueViewModel HotCue3 { get; } = new();
    public HotCueViewModel HotCue4 { get; } = new();
    public HotCueViewModel HotCue5 { get; } = new();
    public HotCueViewModel HotCue6 { get; } = new();
    public HotCueViewModel HotCue7 { get; } = new();
    public HotCueViewModel HotCue8 { get; } = new();

    public string PlayButtonText => IsPlaying ? "⏸" : "▶";
    public string TempoDisplay => $"{(Tempo >= 0 ? "+" : "")}{Tempo:F1}%";

    [RelayCommand]
    private void PlayPause()
    {
        IsPlaying = !IsPlaying;
        OnPropertyChanged(nameof(PlayButtonText));
    }

    [RelayCommand]
    private void SetCue(object? parameter)
    {
        if (parameter is int cueNumber)
        {
            var hotCue = GetHotCue(cueNumber);
            if (!hotCue.IsSet)
            {
                hotCue.IsSet = true;
                hotCue.Label = $"CUE {cueNumber}";
            }
        }
    }

    [RelayCommand]
    private void TriggerCue(object? parameter)
    {
        if (parameter is int cueNumber)
        {
            var hotCue = GetHotCue(cueNumber);
            if (hotCue.IsSet)
            {
                // Jump to cue position
            }
        }
    }

    [RelayCommand]
    private void DeleteCue(object? parameter)
    {
        if (parameter is int cueNumber)
        {
            var hotCue = GetHotCue(cueNumber);
            hotCue.IsSet = false;
            hotCue.Label = string.Empty;
        }
    }

    [RelayCommand]
    private void AutoLoop(object? parameter)
    {
        if (parameter is int beats)
        {
            LoopLengthBeats = beats;
            IsLooping = true;
        }
    }

    [RelayCommand]
    private void HalveLoop()
    {
        if (LoopLengthBeats > 1)
            LoopLengthBeats /= 2;
    }

    [RelayCommand]
    private void DoubleLoop()
    {
        LoopLengthBeats *= 2;
    }

    [RelayCommand]
    private void ExitLoop()
    {
        IsLooping = false;
    }

    [RelayCommand]
    private void Sync()
    {
        IsSynced = !IsSynced;
    }

    private HotCueViewModel GetHotCue(int cueNumber) => cueNumber switch
    {
        1 => HotCue1,
        2 => HotCue2,
        3 => HotCue3,
        4 => HotCue4,
        5 => HotCue5,
        6 => HotCue6,
        7 => HotCue7,
        8 => HotCue8,
        _ => HotCue1
    };
}
