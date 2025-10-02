using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MIXERX.MAUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Services.IAudioEngineService _audioEngine;

    [ObservableProperty]
    private bool is4DeckMode = false;

    public DeckViewModel DeckA { get; }
    public DeckViewModel DeckB { get; }
    public DeckViewModel DeckC { get; }
    public DeckViewModel DeckD { get; }
    public ObservableCollection<DeckViewModel> Decks { get; }

    public MainViewModel(Services.IAudioEngineService audioEngine)
    {
        _audioEngine = audioEngine;
        
        DeckA = new DeckViewModel 
        { 
            DeckName = "DECK 1",
            DeckColor = Color.FromArgb("#00D9FF")
        };
        
        DeckB = new DeckViewModel 
        { 
            DeckName = "DECK 2",
            DeckColor = Color.FromArgb("#FF6B35")
        };

        DeckC = new DeckViewModel 
        { 
            DeckName = "DECK 3",
            DeckColor = Color.FromArgb("#00D9FF")
        };
        
        DeckD = new DeckViewModel 
        { 
            DeckName = "DECK 4",
            DeckColor = Color.FromArgb("#FF6B35")
        };

        Decks = new ObservableCollection<DeckViewModel> { DeckA, DeckB, DeckC, DeckD };
    }

    [RelayCommand]
    private void Toggle4DeckMode()
    {
        Is4DeckMode = !Is4DeckMode;
    }

    [RelayCommand]
    private async Task StartEngine()
    {
        await _audioEngine.StartEngineAsync();
    }

    [RelayCommand]
    private async Task StartRecording()
    {
        // TODO: Implement recording
        await Task.CompletedTask;
    }
}
