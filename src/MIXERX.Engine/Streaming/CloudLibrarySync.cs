using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using MIXERX.Engine.Cues;

namespace MIXERX.Engine.Streaming;

public class CloudLibrarySync
{
    private readonly HttpClient _httpClient;
    private readonly string _apiEndpoint;
    private string? _userToken;
    private IDisposable? _streamingManager;

    public CloudLibrarySync(string apiEndpoint = "https://api.mixerx.cloud")
    {
        _apiEndpoint = apiEndpoint;
        _httpClient = new HttpClient();
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_userToken);

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            var loginData = new
            {
                username = username,
                password = password,
                client = "MIXERX Desktop"
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            
            var response = await _httpClient.PostAsync($"{_apiEndpoint}/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<CloudAuthResponse>(responseJson);
                
                if (authResponse != null)
                {
                    _userToken = authResponse.token;
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Cloud auth error: {ex.Message}");
        }
        
        return false;
    }

    public async Task<bool> SyncLibraryAsync(IEnumerable<LocalTrack> localTracks)
    {
        if (!IsAuthenticated) return false;

        try
        {
            var syncData = new CloudLibraryData
            {
                tracks = localTracks.Select(t => new CloudTrack
                {
                    file_path = t.FilePath,
                    title = t.Title,
                    artist = t.Artist,
                    album = t.Album,
                    bpm = t.Bpm,
                    key = t.Key,
                    genre = t.Genre,
                    duration_ms = (int)t.Duration.TotalMilliseconds,
                    file_hash = CalculateFileHash(t.FilePath),
                    last_played = t.LastPlayed,
                    play_count = t.PlayCount,
                    hot_cues = t.HotCues?.Select(c => new CloudHotCue
                    {
                        number = c.Number,
                        position_ms = (int)c.Position.TotalMilliseconds,
                        label = c.Label,
                        color = c.Color.ToString()
                    }).ToArray()
                }).ToArray(),
                last_sync = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(syncData);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            
            var response = await _httpClient.PostAsync($"{_apiEndpoint}/library/sync", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Library sync error: {ex.Message}");
            return false;
        }
    }

    public async Task<CloudLibraryData?> DownloadLibraryAsync()
    {
        if (!IsAuthenticated) return null;

        try
        {
            var response = await _httpClient.GetAsync($"{_apiEndpoint}/library/download");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CloudLibraryData>(json);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Library download error: {ex.Message}");
        }
        
        return null;
    }

    public async Task<bool> SyncPlaylistsAsync(IEnumerable<LocalPlaylist> playlists)
    {
        if (!IsAuthenticated) return false;

        try
        {
            var playlistData = playlists.Select(p => new CloudPlaylist
            {
                name = p.Name,
                description = p.Description,
                track_ids = p.TrackIds.ToArray(),
                created_date = p.CreatedDate,
                last_modified = p.LastModified
            }).ToArray();

            var json = JsonSerializer.Serialize(new { playlists = playlistData });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
            
            var response = await _httpClient.PostAsync($"{_apiEndpoint}/playlists/sync", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Playlist sync error: {ex.Message}");
            return false;
        }
    }

    public async Task<CloudAnalytics?> GetAnalyticsAsync()
    {
        if (!IsAuthenticated) return null;

        try
        {
            var response = await _httpClient.GetAsync($"{_apiEndpoint}/analytics");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CloudAnalytics>(json);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Analytics error: {ex.Message}");
        }
        
        return null;
    }

    private string CalculateFileHash(string filePath)
    {
        // Simple hash for file identification
        return Path.GetFileName(filePath).GetHashCode().ToString("X");
    }

    public void Dispose()
    {
        _streamingManager?.Dispose();
        _httpClient?.Dispose();
    }
}

// Cloud API Models
public class CloudAuthResponse
{
    public string token { get; set; } = "";
    public string user_id { get; set; } = "";
    public DateTime expires_at { get; set; }
}

public class CloudLibraryData
{
    public CloudTrack[] tracks { get; set; } = Array.Empty<CloudTrack>();
    public DateTime last_sync { get; set; }
}

public class CloudTrack
{
    public string file_path { get; set; } = "";
    public string title { get; set; } = "";
    public string artist { get; set; } = "";
    public string album { get; set; } = "";
    public int bpm { get; set; }
    public string? key { get; set; }
    public string? genre { get; set; }
    public int duration_ms { get; set; }
    public string file_hash { get; set; } = "";
    public DateTime? last_played { get; set; }
    public int play_count { get; set; }
    public CloudHotCue[]? hot_cues { get; set; }
}

public class CloudHotCue
{
    public int number { get; set; }
    public int position_ms { get; set; }
    public string label { get; set; } = "";
    public string color { get; set; } = "";
}

public class CloudPlaylist
{
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string[] track_ids { get; set; } = Array.Empty<string>();
    public DateTime created_date { get; set; }
    public DateTime last_modified { get; set; }
}

public class CloudAnalytics
{
    public int total_tracks { get; set; }
    public int total_play_time_hours { get; set; }
    public string[] top_genres { get; set; } = Array.Empty<string>();
    public string[] top_artists { get; set; } = Array.Empty<string>();
    public CloudPlayStats[] recent_plays { get; set; } = Array.Empty<CloudPlayStats>();
}

public class CloudPlayStats
{
    public string track_title { get; set; } = "";
    public string artist { get; set; } = "";
    public DateTime played_at { get; set; }
    public int play_duration_ms { get; set; }
}

// Local models for sync
public class LocalTrack
{
    public string FilePath { get; set; } = "";
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Album { get; set; } = "";
    public int Bpm { get; set; }
    public string? Key { get; set; }
    public string? Genre { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime? LastPlayed { get; set; }
    public int PlayCount { get; set; }
    public LocalHotCue[]? HotCues { get; set; }
}

public class LocalHotCue
{
    public int Number { get; set; }
    public TimeSpan Position { get; set; }
    public string Label { get; set; } = "";
    public HotCueColor Color { get; set; }
}

public class LocalPlaylist
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> TrackIds { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime LastModified { get; set; }
}
