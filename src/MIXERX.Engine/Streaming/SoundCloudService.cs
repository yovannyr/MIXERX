using System.Text.Json;

namespace MIXERX.Engine.Streaming;

public class SoundCloudService : IStreamingService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;

    public SoundCloudService(string clientId)
    {
        _clientId = clientId;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MIXERX/1.0");
    }

    public string ServiceName => "SoundCloud";
    public bool IsAuthenticated => !string.IsNullOrEmpty(_clientId);

    public async Task<bool> AuthenticateAsync(string? authCode = null)
    {
        // SoundCloud API v2 uses client_id for public access
        return !string.IsNullOrEmpty(_clientId);
    }

    public async Task<StreamingSearchResult> SearchTracksAsync(string query, int limit = 20)
    {
        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://api-v2.soundcloud.com/search/tracks?q={encodedQuery}&client_id={_clientId}&limit={limit}";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new StreamingSearchResult { Error = "SoundCloud search failed" };
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var searchData = JsonSerializer.Deserialize<SoundCloudSearchResponse>(json);
            
            if (searchData?.collection != null)
            {
                var tracks = searchData.collection
                    .Where(item => item.streamable && item.kind == "track")
                    .Select(item => new StreamingTrack
                    {
                        Id = item.id.ToString(),
                        Title = item.title,
                        Artist = item.user?.username ?? "Unknown Artist",
                        Album = "", // SoundCloud doesn't have albums
                        Duration = TimeSpan.FromMilliseconds(item.duration),
                        PreviewUrl = item.stream_url,
                        ImageUrl = item.artwork_url?.Replace("large", "t500x500"), // Higher quality
                        StreamingService = ServiceName,
                        IsStreamable = item.streamable,
                        Popularity = item.playback_count / 1000, // Convert to 0-100 scale
                        Genre = item.genre,
                        Tags = item.tag_list?.Split(' ').Where(t => !string.IsNullOrEmpty(t)).ToArray()
                    }).ToArray();
                
                return new StreamingSearchResult
                {
                    Tracks = tracks,
                    TotalResults = tracks.Length
                };
            }
        }
        catch (Exception ex)
        {
            return new StreamingSearchResult { Error = ex.Message };
        }
        
        return new StreamingSearchResult { Error = "No results found" };
    }

    public async Task<StreamingTrack?> GetTrackDetailsAsync(string trackId)
    {
        try
        {
            var url = $"https://api-v2.soundcloud.com/tracks/{trackId}?client_id={_clientId}";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var track = JsonSerializer.Deserialize<SoundCloudTrack>(json);
            
            if (track != null)
            {
                return new StreamingTrack
                {
                    Id = track.id.ToString(),
                    Title = track.title,
                    Artist = track.user?.username ?? "Unknown Artist",
                    Album = "",
                    Duration = TimeSpan.FromMilliseconds(track.duration),
                    PreviewUrl = track.stream_url,
                    ImageUrl = track.artwork_url?.Replace("large", "t500x500"),
                    StreamingService = ServiceName,
                    IsStreamable = track.streamable,
                    Popularity = track.playback_count / 1000,
                    Genre = track.genre,
                    Tags = track.tag_list?.Split(' ').Where(t => !string.IsNullOrEmpty(t)).ToArray(),
                    Description = track.description
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get SoundCloud track error: {ex.Message}");
        }
        
        return null;
    }

    public async Task<string?> GetStreamUrlAsync(string trackId)
    {
        try
        {
            var url = $"https://api-v2.soundcloud.com/tracks/{trackId}/stream?client_id={_clientId}";
            
            // SoundCloud returns a redirect to the actual stream URL
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await _httpClient.SendAsync(request);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                response.StatusCode == System.Net.HttpStatusCode.Found)
            {
                return response.Headers.Location?.ToString();
            }
            
            return url; // Fallback to original URL
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get SoundCloud stream URL error: {ex.Message}");
            return null;
        }
    }

    public async Task<StreamingPlaylist[]> GetPlaylistsAsync()
    {
        try
        {
            // Get featured/trending playlists
            var url = $"https://api-v2.soundcloud.com/search/playlists?q=electronic&client_id={_clientId}&limit=20";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Array.Empty<StreamingPlaylist>();
            
            var json = await response.Content.ReadAsStringAsync();
            var playlistData = JsonSerializer.Deserialize<SoundCloudSearchResponse>(json);
            
            if (playlistData?.collection != null)
            {
                return playlistData.collection
                    .Where(item => item.kind == "playlist")
                    .Select(p => new StreamingPlaylist
                    {
                        Id = p.id.ToString(),
                        Name = p.title,
                        Description = p.description ?? "",
                        TrackCount = p.track_count,
                        ImageUrl = p.artwork_url?.Replace("large", "t500x500"),
                        StreamingService = ServiceName,
                        CreatedBy = p.user?.username ?? "Unknown"
                    }).ToArray();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get SoundCloud playlists error: {ex.Message}");
        }
        
        return Array.Empty<StreamingPlaylist>();
    }

    public async Task<StreamingTrack[]> GetPlaylistTracksAsync(string playlistId)
    {
        try
        {
            var url = $"https://api-v2.soundcloud.com/playlists/{playlistId}?client_id={_clientId}";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Array.Empty<StreamingTrack>();
            
            var json = await response.Content.ReadAsStringAsync();
            var playlist = JsonSerializer.Deserialize<SoundCloudPlaylist>(json);
            
            if (playlist?.tracks != null)
            {
                return playlist.tracks
                    .Where(t => t.streamable)
                    .Select(track => new StreamingTrack
                    {
                        Id = track.id.ToString(),
                        Title = track.title,
                        Artist = track.user?.username ?? "Unknown Artist",
                        Duration = TimeSpan.FromMilliseconds(track.duration),
                        PreviewUrl = track.stream_url,
                        ImageUrl = track.artwork_url?.Replace("large", "t500x500"),
                        StreamingService = ServiceName,
                        IsStreamable = track.streamable,
                        Genre = track.genre
                    }).ToArray();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get playlist tracks error: {ex.Message}");
        }
        
        return Array.Empty<StreamingTrack>();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// SoundCloud API Response Models
public class SoundCloudSearchResponse
{
    public SoundCloudTrack[] collection { get; set; } = Array.Empty<SoundCloudTrack>();
    public int total_results { get; set; }
}

public class SoundCloudTrack
{
    public int id { get; set; }
    public string title { get; set; } = "";
    public string kind { get; set; } = "";
    public int duration { get; set; }
    public bool streamable { get; set; }
    public string? stream_url { get; set; }
    public string? artwork_url { get; set; }
    public int playback_count { get; set; }
    public string? genre { get; set; }
    public string? tag_list { get; set; }
    public string? description { get; set; }
    public int track_count { get; set; }
    public SoundCloudUser? user { get; set; }
    public SoundCloudTrack[]? tracks { get; set; }
}

public class SoundCloudUser
{
    public int id { get; set; }
    public string username { get; set; } = "";
    public string? avatar_url { get; set; }
}

public class SoundCloudPlaylist
{
    public int id { get; set; }
    public string title { get; set; } = "";
    public string? description { get; set; }
    public int track_count { get; set; }
    public string? artwork_url { get; set; }
    public SoundCloudUser? user { get; set; }
    public SoundCloudTrack[]? tracks { get; set; }
}
