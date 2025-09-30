using System.Text.Json;

namespace MIXERX.Engine.Streaming;

public class BeatportService : IStreamingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private string? _accessToken;

    public BeatportService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MIXERX/1.0");
    }

    public string ServiceName => "Beatport";
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

    public async Task<bool> AuthenticateAsync(string? authCode = null)
    {
        try
        {
            // Beatport LINK authentication (simplified)
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.beatport.com/v4/auth/o/token/");
            
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _apiKey}
            };
            
            request.Content = new FormUrlEncodedContent(parameters);
            
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<BeatportTokenResponse>(json);
                
                if (tokenData != null)
                {
                    _accessToken = tokenData.access_token;
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Beatport auth error: {ex.Message}");
        }
        
        return false;
    }

    public async Task<StreamingSearchResult> SearchTracksAsync(string query, int limit = 20)
    {
        if (!IsAuthenticated) await AuthenticateAsync();

        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://api.beatport.com/v4/catalog/search/?q={encodedQuery}&type=track&per_page={limit}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new StreamingSearchResult { Error = "Beatport search failed" };
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var searchData = JsonSerializer.Deserialize<BeatportSearchResponse>(json);
            
            if (searchData?.tracks != null)
            {
                var tracks = searchData.tracks.Select(item => new StreamingTrack
                {
                    Id = item.id.ToString(),
                    Title = item.name,
                    Artist = string.Join(", ", item.artists.Select(a => a.name)),
                    Album = item.release?.name ?? "",
                    Duration = TimeSpan.FromSeconds(item.length_ms / 1000),
                    PreviewUrl = item.sample_url,
                    ImageUrl = item.image?.uri,
                    StreamingService = ServiceName,
                    IsStreamable = !string.IsNullOrEmpty(item.sample_url),
                    Bpm = item.bpm,
                    Key = item.key?.name,
                    Genre = item.genre?.name,
                    Label = item.label?.name,
                    ReleaseDate = item.release?.new_release_date,
                    Price = item.price?.value
                }).ToArray();
                
                return new StreamingSearchResult
                {
                    Tracks = tracks,
                    TotalResults = searchData.count
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
        if (!IsAuthenticated) await AuthenticateAsync();

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.beatport.com/v4/catalog/tracks/{trackId}/");
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var track = JsonSerializer.Deserialize<BeatportTrack>(json);
            
            if (track != null)
            {
                return new StreamingTrack
                {
                    Id = track.id.ToString(),
                    Title = track.name,
                    Artist = string.Join(", ", track.artists.Select(a => a.name)),
                    Album = track.release?.name ?? "",
                    Duration = TimeSpan.FromSeconds(track.length_ms / 1000),
                    PreviewUrl = track.sample_url,
                    ImageUrl = track.image?.uri,
                    StreamingService = ServiceName,
                    IsStreamable = !string.IsNullOrEmpty(track.sample_url),
                    Bpm = track.bpm,
                    Key = track.key?.name,
                    Genre = track.genre?.name,
                    Label = track.label?.name,
                    ReleaseDate = track.release?.new_release_date,
                    Price = track.price?.value
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get Beatport track error: {ex.Message}");
        }
        
        return null;
    }

    public async Task<string?> GetStreamUrlAsync(string trackId)
    {
        var track = await GetTrackDetailsAsync(trackId);
        return track?.PreviewUrl; // Preview only - full tracks require purchase
    }

    public async Task<StreamingPlaylist[]> GetPlaylistsAsync()
    {
        if (!IsAuthenticated) await AuthenticateAsync();

        try
        {
            // Get featured charts/playlists
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.beatport.com/v4/catalog/charts/?per_page=20");
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return Array.Empty<StreamingPlaylist>();
            
            var json = await response.Content.ReadAsStringAsync();
            var chartData = JsonSerializer.Deserialize<BeatportChartsResponse>(json);
            
            if (chartData?.results != null)
            {
                return chartData.results.Select(chart => new StreamingPlaylist
                {
                    Id = chart.id.ToString(),
                    Name = chart.name,
                    Description = chart.description ?? "",
                    TrackCount = chart.track_count,
                    ImageUrl = chart.image?.uri,
                    StreamingService = ServiceName,
                    Genre = chart.genre?.name
                }).ToArray();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get Beatport charts error: {ex.Message}");
        }
        
        return Array.Empty<StreamingPlaylist>();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Beatport API Response Models
public class BeatportTokenResponse
{
    public string access_token { get; set; } = "";
    public string token_type { get; set; } = "";
    public int expires_in { get; set; }
}

public class BeatportSearchResponse
{
    public BeatportTrack[] tracks { get; set; } = Array.Empty<BeatportTrack>();
    public int count { get; set; }
}

public class BeatportTrack
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public BeatportArtist[] artists { get; set; } = Array.Empty<BeatportArtist>();
    public BeatportRelease? release { get; set; }
    public int length_ms { get; set; }
    public int bpm { get; set; }
    public string? sample_url { get; set; }
    public BeatportImage? image { get; set; }
    public BeatportKey? key { get; set; }
    public BeatportGenre? genre { get; set; }
    public BeatportLabel? label { get; set; }
    public BeatportPrice? price { get; set; }
}

public class BeatportArtist
{
    public int id { get; set; }
    public string name { get; set; } = "";
}

public class BeatportRelease
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string? new_release_date { get; set; }
}

public class BeatportImage
{
    public string uri { get; set; } = "";
}

public class BeatportKey
{
    public int id { get; set; }
    public string name { get; set; } = "";
}

public class BeatportGenre
{
    public int id { get; set; }
    public string name { get; set; } = "";
}

public class BeatportLabel
{
    public int id { get; set; }
    public string name { get; set; } = "";
}

public class BeatportPrice
{
    public decimal value { get; set; }
    public string currency { get; set; } = "";
}

public class BeatportChartsResponse
{
    public BeatportChart[] results { get; set; } = Array.Empty<BeatportChart>();
}

public class BeatportChart
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string? description { get; set; }
    public int track_count { get; set; }
    public BeatportImage? image { get; set; }
    public BeatportGenre? genre { get; set; }
}
