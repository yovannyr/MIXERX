using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Services.IEngineService _engineService;

    public DeckViewModel DeckA { get; }
    public DeckViewModel DeckB { get; }

    public MainViewModel(Services.IEngineService engineService)
    {
        _engineService = engineService;
        
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
        await _engineService.StartEngineAsync();
    }

    [RelayCommand]
    private async Task StartRecording()
    {
        // TODO: Implement recording
        await Task.CompletedTask;
    }
}
