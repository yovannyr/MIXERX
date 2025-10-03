using System.Text.Json;
using System.Text;

namespace MIXERX.Infrastructure.Services.Streaming;

public class SpotifyService : IStreamingService
{
    private readonly HttpClient _httpClient;
    private string? _accessToken;
    private DateTime _tokenExpiry;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public SpotifyService(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MIXERX/1.0");
    }

    public string ServiceName => "Spotify";
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && DateTime.Now < _tokenExpiry;

    public async Task<bool> AuthenticateAsync(string? authCode = null)
    {
        try
        {
            // Client Credentials Flow for public data
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Add("Authorization", $"Basic {authString}");
            
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            };
            
            request.Content = new FormUrlEncodedContent(parameters);
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return false;
            
            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<SpotifyTokenResponse>(json);
            
            if (tokenData != null)
            {
                _accessToken = tokenData.access_token;
                _tokenExpiry = DateTime.Now.AddSeconds(tokenData.expires_in - 60); // 1 minute buffer
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Spotify auth error: {ex.Message}");
        }
        
        return false;
    }

    public async Task<StreamingSearchResult> SearchTracksAsync(string query, int limit = 20)
    {
        if (!IsAuthenticated)
        {
            await AuthenticateAsync();
        }

        try
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://api.spotify.com/v1/search?q={encodedQuery}&type=track&limit={limit}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new StreamingSearchResult { Error = "Search failed" };
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var searchData = JsonSerializer.Deserialize<SpotifySearchResponse>(json);
            
            if (searchData?.tracks?.items != null)
            {
                var tracks = searchData.tracks.items.Select(item => new StreamingTrack
                {
                    Id = item.id,
                    Title = item.name,
                    Artist = string.Join(", ", item.artists.Select(a => a.name)),
                    Album = item.album.name,
                    Duration = TimeSpan.FromMilliseconds(item.duration_ms),
                    PreviewUrl = item.preview_url,
                    ImageUrl = item.album.images?.FirstOrDefault()?.url,
                    StreamingService = ServiceName,
                    IsStreamable = !string.IsNullOrEmpty(item.preview_url),
                    Popularity = item.popularity
                }).ToArray();
                
                return new StreamingSearchResult
                {
                    Tracks = tracks,
                    TotalResults = searchData.tracks.total
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/tracks/{trackId}");
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var track = JsonSerializer.Deserialize<SpotifyTrack>(json);
            
            if (track != null)
            {
                return new StreamingTrack
                {
                    Id = track.id,
                    Title = track.name,
                    Artist = string.Join(", ", track.artists.Select(a => a.name)),
                    Album = track.album.name,
                    Duration = TimeSpan.FromMilliseconds(track.duration_ms),
                    PreviewUrl = track.preview_url,
                    ImageUrl = track.album.images?.FirstOrDefault()?.url,
                    StreamingService = ServiceName,
                    IsStreamable = !string.IsNullOrEmpty(track.preview_url),
                    Popularity = track.popularity
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get track error: {ex.Message}");
        }
        
        return null;
    }

    public async Task<string?> GetStreamUrlAsync(string trackId)
    {
        var track = await GetTrackDetailsAsync(trackId);
        return track?.PreviewUrl; // 30-second preview only for free tier
    }

    public async Task<StreamingPlaylist[]> GetPlaylistsAsync()
    {
        // Featured playlists (public)
        if (!IsAuthenticated) await AuthenticateAsync();

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/browse/featured-playlists?limit=20");
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return Array.Empty<StreamingPlaylist>();
            
            var json = await response.Content.ReadAsStringAsync();
            var playlistData = JsonSerializer.Deserialize<SpotifyPlaylistResponse>(json);
            
            if (playlistData?.playlists?.items != null)
            {
                return playlistData.playlists.items.Select(p => new StreamingPlaylist
                {
                    Id = p.id,
                    Name = p.name,
                    Description = p.description,
                    TrackCount = p.tracks.total,
                    ImageUrl = p.images?.FirstOrDefault()?.url,
                    StreamingService = ServiceName
                }).ToArray();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get playlists error: {ex.Message}");
        }
        
        return Array.Empty<StreamingPlaylist>();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Spotify API Response Models
public class SpotifyTokenResponse
{
    public string access_token { get; set; } = "";
    public string token_type { get; set; } = "";
    public int expires_in { get; set; }
}

public class SpotifySearchResponse
{
    public SpotifyTracksResponse? tracks { get; set; }
}

public class SpotifyTracksResponse
{
    public SpotifyTrack[] items { get; set; } = Array.Empty<SpotifyTrack>();
    public int total { get; set; }
}

public class SpotifyTrack
{
    public string id { get; set; } = "";
    public string name { get; set; } = "";
    public SpotifyArtist[] artists { get; set; } = Array.Empty<SpotifyArtist>();
    public SpotifyAlbum album { get; set; } = new();
    public int duration_ms { get; set; }
    public string? preview_url { get; set; }
    public int popularity { get; set; }
}

public class SpotifyArtist
{
    public string id { get; set; } = "";
    public string name { get; set; } = "";
}

public class SpotifyAlbum
{
    public string id { get; set; } = "";
    public string name { get; set; } = "";
    public SpotifyImage[]? images { get; set; }
}

public class SpotifyImage
{
    public string url { get; set; } = "";
    public int width { get; set; }
    public int height { get; set; }
}

public class SpotifyPlaylistResponse
{
    public SpotifyPlaylistsContainer? playlists { get; set; }
}

public class SpotifyPlaylistsContainer
{
    public SpotifyPlaylist[] items { get; set; } = Array.Empty<SpotifyPlaylist>();
}

public class SpotifyPlaylist
{
    public string id { get; set; } = "";
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public SpotifyTracksInfo tracks { get; set; } = new();
    public SpotifyImage[]? images { get; set; }
}

public class SpotifyTracksInfo
{
    public int total { get; set; }
}
