namespace MIXERX.Core.Models;

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