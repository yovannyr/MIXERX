namespace MIXERX.Core.Models;

public class StreamingSearchResult
{
    public StreamingTrack[] Tracks { get; set; } = Array.Empty<StreamingTrack>();
    public int TotalResults { get; set; }
    public string? Error { get; set; }
    public string? NextPageToken { get; set; }
}