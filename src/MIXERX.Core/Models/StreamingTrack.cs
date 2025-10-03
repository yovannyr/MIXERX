namespace MIXERX.Core.Models;

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