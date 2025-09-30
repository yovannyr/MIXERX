using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using MIXERX.Engine.Streaming;

namespace MIXERX.UI.ViewModels;

public class StreamingViewModel : ViewModelBase
{
    private readonly StreamingManager _streamingManager;
    private string _searchQuery = "";
    private bool _isSearching;
    private StreamingTrack? _selectedTrack;
    private string _selectedService = "All Services";

    public StreamingViewModel()
    {
        _streamingManager = new StreamingManager();
        
        // Initialize streaming services
        InitializeServices();
        
        SearchTracks = new ObservableCollection<StreamingTrackViewModel>();
        Playlists = new ObservableCollection<StreamingPlaylistViewModel>();
        
        SearchCommand = ReactiveCommand.CreateFromTask(PerformSearch);
        LoadTrackCommand = ReactiveCommand.CreateFromTask<StreamingTrack>(LoadTrack);
        PreviewTrackCommand = ReactiveCommand.CreateFromTask<StreamingTrack>(PreviewTrack);
        RefreshPlaylistsCommand = ReactiveCommand.CreateFromTask(RefreshPlaylists);
        
        // Auto-authenticate services
        _ = Task.Run(async () => await _streamingManager.AuthenticateAllServicesAsync());
    }

    public ObservableCollection<StreamingTrackViewModel> SearchTracks { get; }
    public ObservableCollection<StreamingPlaylistViewModel> Playlists { get; }

    public string SearchQuery
    {
        get => _searchQuery;
        set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
    }

    public bool IsSearching
    {
        get => _isSearching;
        set => this.RaiseAndSetIfChanged(ref _isSearching, value);
    }

    public StreamingTrack? SelectedTrack
    {
        get => _selectedTrack;
        set => this.RaiseAndSetIfChanged(ref _selectedTrack, value);
    }

    public string SelectedService
    {
        get => _selectedService;
        set => this.RaiseAndSetIfChanged(ref _selectedService, value);
    }

    public string[] AvailableServices => new[] { "All Services" }
        .Concat(_streamingManager.GetAvailableServices())
        .ToArray();

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<StreamingTrack, Unit> LoadTrackCommand { get; }
    public ReactiveCommand<StreamingTrack, Unit> PreviewTrackCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshPlaylistsCommand { get; }

    private void InitializeServices()
    {
        // Initialize with demo credentials (in production, use secure config)
        var spotifyService = new SpotifyService("demo_client_id", "demo_client_secret");
        var soundcloudService = new SoundCloudService("demo_client_id");
        var beatportService = new BeatportService("demo_api_key");
        
        _streamingManager.RegisterService(spotifyService);
        _streamingManager.RegisterService(soundcloudService);
        _streamingManager.RegisterService(beatportService);
    }

    private async Task PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        IsSearching = true;
        SearchTracks.Clear();

        try
        {
            StreamingSearchResult result;
            
            if (SelectedService == "All Services")
            {
                result = await _streamingManager.SearchAllServicesAsync(SearchQuery);
            }
            else
            {
                var service = _streamingManager.GetService(SelectedService);
                result = service != null 
                    ? await service.SearchTracksAsync(SearchQuery)
                    : new StreamingSearchResult { Error = "Service not available" };
            }

            if (result.Error == null)
            {
                foreach (var track in result.Tracks)
                {
                    SearchTracks.Add(new StreamingTrackViewModel(track));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search exception: {ex.Message}");
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async Task LoadTrack(StreamingTrack track)
    {
        try
        {
            var streamUrl = await GetStreamUrl(track);
            if (!string.IsNullOrEmpty(streamUrl))
            {
                // Load track into active deck
                // This would integrate with the main DJ interface
                System.Diagnostics.Debug.WriteLine($"Loading streaming track: {track.Title} by {track.Artist}");
                
                // In production, this would:
                // 1. Download/cache the track
                // 2. Load into selected deck
                // 3. Update deck UI with track info
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load track error: {ex.Message}");
        }
    }

    private async Task PreviewTrack(StreamingTrack track)
    {
        try
        {
            var previewUrl = await GetStreamUrl(track);
            if (!string.IsNullOrEmpty(previewUrl))
            {
                // Play preview in separate audio channel
                System.Diagnostics.Debug.WriteLine($"Previewing: {track.Title}");
                
                // In production, this would:
                // 1. Stream preview audio
                // 2. Play through headphone channel
                // 3. Show preview controls
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Preview error: {ex.Message}");
        }
    }

    private async Task RefreshPlaylists()
    {
        Playlists.Clear();

        try
        {
            foreach (var service in _streamingManager.GetAvailableServices())
            {
                var streamingService = _streamingManager.GetService(service);
                if (streamingService?.IsAuthenticated == true)
                {
                    var playlists = await streamingService.GetPlaylistsAsync();
                    foreach (var playlist in playlists)
                    {
                        Playlists.Add(new StreamingPlaylistViewModel(playlist));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Refresh playlists error: {ex.Message}");
        }
    }

    private async Task<string?> GetStreamUrl(StreamingTrack track)
    {
        var service = _streamingManager.GetService(track.StreamingService);
        return service != null ? await service.GetStreamUrlAsync(track.Id) : null;
    }

    public async Task<StreamingTrack[]> GetRecommendationsForTrack(StreamingTrack track)
    {
        return await _streamingManager.GetRecommendationsAsync(track);
    }
}

public class StreamingTrackViewModel : ViewModelBase
{
    public StreamingTrack Track { get; }

    public StreamingTrackViewModel(StreamingTrack track)
    {
        Track = track;
    }

    public string DisplayTitle => Track.Title;
    public string DisplayArtist => Track.Artist;
    public string DisplayDuration => Track.Duration.ToString(@"mm\:ss");
    public string DisplayBpm => Track.Bpm > 0 ? $"{Track.Bpm} BPM" : "";
    public string DisplayKey => Track.Key ?? "";
    public string DisplayService => Track.StreamingService;
    public string DisplayGenre => Track.Genre ?? "";
    public bool HasPreview => !string.IsNullOrEmpty(Track.PreviewUrl);
    public bool IsStreamable => Track.IsStreamable;
}

public class StreamingPlaylistViewModel : ViewModelBase
{
    public StreamingPlaylist Playlist { get; }

    public StreamingPlaylistViewModel(StreamingPlaylist playlist)
    {
        Playlist = playlist;
    }

    public string DisplayName => Playlist.Name;
    public string DisplayDescription => Playlist.Description;
    public string DisplayTrackCount => $"{Playlist.TrackCount} tracks";
    public string DisplayService => Playlist.StreamingService;
    public string DisplayGenre => Playlist.Genre ?? "";
}
