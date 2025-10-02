namespace MIXERX.Engine.Streaming;

public interface IStreamingService : IDisposable
{
    string ServiceName { get; }
    bool IsAuthenticated { get; }
    
    Task<bool> AuthenticateAsync(string? authCode = null);
    Task<StreamingSearchResult> SearchTracksAsync(string query, int limit = 20);
    Task<StreamingTrack?> GetTrackDetailsAsync(string trackId);
    Task<string?> GetStreamUrlAsync(string trackId);
    Task<StreamingPlaylist[]> GetPlaylistsAsync();
}

public class StreamingSearchResult
{
    public StreamingTrack[] Tracks { get; set; } = Array.Empty<StreamingTrack>();
    public int TotalResults { get; set; }
    public string? Error { get; set; }
    public string? NextPageToken { get; set; }
}

public class StreamingTrack
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Album { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public string? PreviewUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string StreamingService { get; set; } = "";
    public bool IsStreamable { get; set; }
    public int Popularity { get; set; }
    
    // DJ-specific metadata
    public int Bpm { get; set; }
    public string? Key { get; set; }
    public string? Genre { get; set; }
    public string? Label { get; set; }
    public string? ReleaseDate { get; set; }
    public decimal? Price { get; set; }
    public string[]? Tags { get; set; }
    public string? Description { get; set; }
    
    // Streaming quality info
    public StreamingQuality Quality { get; set; } = StreamingQuality.Preview;
    public bool RequiresPurchase { get; set; }
    public bool RequiresSubscription { get; set; }
}

public class StreamingPlaylist
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int TrackCount { get; set; }
    public string? ImageUrl { get; set; }
    public string StreamingService { get; set; } = "";
    public string? CreatedBy { get; set; }
    public string? Genre { get; set; }
    public DateTime? LastUpdated { get; set; }
}

public enum StreamingQuality
{
    Preview,    // 30-second preview
    Low,        // 128 kbps
    Medium,     // 256 kbps
    High,       // 320 kbps
    Lossless    // FLAC/WAV
}
