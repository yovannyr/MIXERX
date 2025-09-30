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

public class StreamingManager
{
    private readonly Dictionary<string, IStreamingService> _services = new();
    private readonly List<StreamingTrack> _cache = new();

    public void RegisterService(IStreamingService service)
    {
        _services[service.ServiceName] = service;
    }

    public async Task<StreamingSearchResult> SearchAllServicesAsync(string query, int limitPerService = 10)
    {
        var allTracks = new List<StreamingTrack>();
        var tasks = new List<Task<StreamingSearchResult>>();

        foreach (var service in _services.Values)
        {
            if (service.IsAuthenticated)
            {
                tasks.Add(service.SearchTracksAsync(query, limitPerService));
            }
        }

        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            if (result.Error == null)
            {
                allTracks.AddRange(result.Tracks);
            }
        }

        // Sort by relevance (popularity, exact matches, etc.)
        var sortedTracks = allTracks
            .OrderByDescending(t => t.Popularity)
            .ThenByDescending(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return new StreamingSearchResult
        {
            Tracks = sortedTracks,
            TotalResults = sortedTracks.Length
        };
    }

    public async Task<StreamingTrack[]> GetRecommendationsAsync(StreamingTrack currentTrack)
    {
        var recommendations = new List<StreamingTrack>();

        // Search for similar tracks across all services
        var searchQueries = new[]
        {
            $"{currentTrack.Artist}",
            $"genre:{currentTrack.Genre}",
            $"bpm:{currentTrack.Bpm}",
            $"key:{currentTrack.Key}"
        };

        foreach (var query in searchQueries.Where(q => !string.IsNullOrEmpty(q)))
        {
            var results = await SearchAllServicesAsync(query, 5);
            recommendations.AddRange(results.Tracks.Where(t => t.Id != currentTrack.Id));
        }

        // Remove duplicates and sort by compatibility
        return recommendations
            .GroupBy(t => $"{t.Title}-{t.Artist}")
            .Select(g => g.First())
            .OrderByDescending(t => CalculateCompatibility(currentTrack, t))
            .Take(20)
            .ToArray();
    }

    private float CalculateCompatibility(StreamingTrack current, StreamingTrack candidate)
    {
        var score = 0.0f;

        // BPM compatibility
        if (current.Bpm > 0 && candidate.Bpm > 0)
        {
            var bpmDiff = Math.Abs(current.Bpm - candidate.Bpm);
            score += bpmDiff < 5 ? 0.3f : bpmDiff < 10 ? 0.2f : 0.1f;
        }

        // Key compatibility
        if (!string.IsNullOrEmpty(current.Key) && !string.IsNullOrEmpty(candidate.Key))
        {
            score += current.Key == candidate.Key ? 0.3f : 0.1f;
        }

        // Genre compatibility
        if (!string.IsNullOrEmpty(current.Genre) && !string.IsNullOrEmpty(candidate.Genre))
        {
            score += current.Genre == candidate.Genre ? 0.2f : 0.1f;
        }

        // Artist similarity
        if (current.Artist == candidate.Artist)
        {
            score += 0.2f;
        }

        return score;
    }

    public IStreamingService? GetService(string serviceName)
    {
        return _services.GetValueOrDefault(serviceName);
    }

    public string[] GetAvailableServices()
    {
        return _services.Keys.ToArray();
    }

    public async Task<bool> AuthenticateAllServicesAsync()
    {
        var tasks = _services.Values.Select(s => s.AuthenticateAsync());
        var results = await Task.WhenAll(tasks);
        return results.Any(r => r);
    }

    public void Dispose()
    {
        foreach (var service in _services.Values)
        {
            service.Dispose();
        }
        _services.Clear();
    }
}
