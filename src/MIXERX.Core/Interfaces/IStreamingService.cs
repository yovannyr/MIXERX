namespace MIXERX.Core.Interfaces;

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