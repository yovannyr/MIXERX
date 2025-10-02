using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MIXERX.MAUI.ViewModels;

public partial class StreamingViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private string selectedService = "Spotify";

    public ObservableCollection<string> Services { get; } = new() 
    { 
        "Spotify", "Tidal", "Apple Music", "Beatport", "SoundCloud" 
    };

    public ObservableCollection<TrackViewModel> Results { get; } = new();

    [RelayCommand]
    private async Task SearchAsync()
    {
        Results.Clear();
        // TODO: Call streaming service
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        // TODO: OAuth flow
        await Task.CompletedTask;
    }
}
