using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MIXERX.MAUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Services.IEngineService _engineService;

    public MainViewModel(Services.IEngineService engineService)
    {
        _engineService = engineService;
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
