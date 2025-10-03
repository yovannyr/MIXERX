using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIXERX.MAUI.Models;

namespace MIXERX.MAUI.ViewModels;

public partial class StemsViewModel : ObservableObject
{
    [ObservableProperty]
    private StemControl vocals = new() { Type = StemType.Vocals };

    [ObservableProperty]
    private StemControl melody = new() { Type = StemType.Melody };

    [ObservableProperty]
    private StemControl bass = new() { Type = StemType.Bass };

    [ObservableProperty]
    private StemControl drums = new() { Type = StemType.Drums };

    [RelayCommand]
    private void ToggleMute(StemType stem)
    {
        GetStem(stem).IsMuted = !GetStem(stem).IsMuted;
    }

    [RelayCommand]
    private void ToggleSolo(StemType stem)
    {
        GetStem(stem).IsSolo = !GetStem(stem).IsSolo;
    }

    [RelayCommand]
    private void Acapella()
    {
        Vocals.IsMuted = false;
        Melody.IsMuted = true;
        Bass.IsMuted = true;
        Drums.IsMuted = true;
    }

    [RelayCommand]
    private void Instrumental()
    {
        Vocals.IsMuted = true;
        Melody.IsMuted = false;
        Bass.IsMuted = false;
        Drums.IsMuted = false;
    }

    private StemControl GetStem(StemType type) => type switch
    {
        StemType.Vocals => Vocals,
        StemType.Melody => Melody,
        StemType.Bass => Bass,
        StemType.Drums => Drums,
        _ => Vocals
    };
}
