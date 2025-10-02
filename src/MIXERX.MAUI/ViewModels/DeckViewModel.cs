using CommunityToolkit.Mvvm.ComponentModel;

namespace MIXERX.MAUI.ViewModels;

public partial class DeckViewModel : ObservableObject
{
    [ObservableProperty]
    private string trackName = "No Track Loaded";

    [ObservableProperty]
    private string bpm = "120.0 BPM";
}
