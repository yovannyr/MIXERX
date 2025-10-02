using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Services.IAudioEngineService _audioEngine;

    public DeckViewModel DeckA { get; }
    public DeckViewModel DeckB { get; }

    public MainViewModel(Services.IAudioEngineService audioEngine)
    {
        _audioEngine = audioEngine;
        
        DeckA = new DeckViewModel 
        { 
            DeckName = "DECK A",
            DeckColor = Color.FromArgb("#00D9FF")
        };
        
        DeckB = new DeckViewModel 
        { 
            DeckName = "DECK B",
            DeckColor = Color.FromArgb("#FF6B35")
        };
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
