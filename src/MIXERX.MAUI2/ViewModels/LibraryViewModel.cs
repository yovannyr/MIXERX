using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MIXERX.MAUI.ViewModels;

public partial class TrackViewModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string filePath = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string artist = string.Empty;

    [ObservableProperty]
    private string album = string.Empty;

    [ObservableProperty]
    private TimeSpan duration;

    [ObservableProperty]
    private double? bpm;

    [ObservableProperty]
    private string? key;

    public string DisplayTitle => !string.IsNullOrEmpty(Title) ? Title : Path.GetFileNameWithoutExtension(FilePath);
    public string DisplayArtist => !string.IsNullOrEmpty(Artist) ? Artist : "Unknown Artist";
    public string DisplayBpm => Bpm.HasValue ? $"{Bpm:F1}" : "--";
    public string DisplayKey => Key ?? "--";
    public string DisplayDuration => Duration.ToString(@"mm\:ss");
}

public partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private TrackViewModel? selectedTrack;

    [ObservableProperty]
    private bool isAutoplayEnabled;

    public ObservableCollection<TrackViewModel> Tracks { get; } = new();

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        // TODO: Implement search with LibraryService
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ImportDirectoryAsync()
    {
        // TODO: Implement directory picker
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void LoadTrack(TrackViewModel? track)
    {
        if (track != null)
        {
            SelectedTrack = track;
            // TODO: Load track to deck
        }
    }

    [RelayCommand]
    private async Task LoadAllTracksAsync()
    {
        // Mock data for now
        Tracks.Clear();
        Tracks.Add(new TrackViewModel { Title = "Track 1", Artist = "Artist 1", Bpm = 128, Key = "Am", Duration = TimeSpan.FromMinutes(3.5) });
        Tracks.Add(new TrackViewModel { Title = "Track 2", Artist = "Artist 2", Bpm = 140, Key = "Cm", Duration = TimeSpan.FromMinutes(4.2) });
        Tracks.Add(new TrackViewModel { Title = "Track 3", Artist = "Artist 3", Bpm = 120, Key = "G", Duration = TimeSpan.FromMinutes(5.1) });
        await Task.CompletedTask;
    }

    public LibraryViewModel()
    {
        _ = LoadAllTracksAsync();
    }
}
