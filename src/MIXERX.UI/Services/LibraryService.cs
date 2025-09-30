using Microsoft.EntityFrameworkCore;
using MIXERX.Core.Models;
using MIXERX.UI.Data;

namespace MIXERX.UI.Services;

public interface ILibraryService
{
    Task<IEnumerable<Track>> SearchAsync(string query);
    Task<Track> AnalyzeTrackAsync(string filePath);
    Task ImportDirectoryAsync(string path);
    Task<IEnumerable<Track>> GetAllTracksAsync();
}

public class LibraryService : ILibraryService
{
    private readonly LibraryContext _context;

    public LibraryService()
    {
        _context = new LibraryContext();
        _context.Database.EnsureCreated();
    }

    public async Task<IEnumerable<Track>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllTracksAsync();

        return await _context.Tracks
            .Where(t => t.Title.Contains(query) || t.Artist.Contains(query) || t.Album.Contains(query))
            .ToListAsync();
    }

    public async Task<Track> AnalyzeTrackAsync(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        
        var track = new Track
        {
            FilePath = filePath,
            Title = Path.GetFileNameWithoutExtension(filePath),
            Artist = "Unknown Artist",
            Album = "Unknown Album",
            Duration = TimeSpan.Zero,
            Bpm = EstimateBpm(filePath), // Basic BPM estimation
            Key = "Unknown",
            LastModified = fileInfo.LastWriteTime
        };

        _context.Tracks.Add(track);
        await _context.SaveChangesAsync();
        
        return track;
    }

    private float EstimateBpm(string filePath)
    {
        // Simple BPM estimation based on filename patterns or default ranges
        var fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
        
        // Look for BPM in filename (e.g., "Song - 128 BPM.mp3")
        var bpmMatch = System.Text.RegularExpressions.Regex.Match(fileName, @"(\d{2,3})\s*bpm");
        if (bpmMatch.Success && float.TryParse(bpmMatch.Groups[1].Value, out var bpm))
        {
            return bpm;
        }
        
        // Genre-based BPM estimation
        if (fileName.Contains("house")) return 128f;
        if (fileName.Contains("techno")) return 130f;
        if (fileName.Contains("trance")) return 138f;
        if (fileName.Contains("drum") || fileName.Contains("bass")) return 174f;
        if (fileName.Contains("hip") || fileName.Contains("hop")) return 90f;
        
        // Default BPM for unknown tracks
        return 120f;
    }

    public async Task ImportDirectoryAsync(string path)
    {
        var audioExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg", ".m4a" };
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(f => audioExtensions.Contains(Path.GetExtension(f).ToLower()));

        foreach (var file in files)
        {
            var exists = await _context.Tracks.AnyAsync(t => t.FilePath == file);
            if (!exists)
            {
                await AnalyzeTrackAsync(file);
            }
        }
    }

    public async Task<IEnumerable<Track>> GetAllTracksAsync()
    {
        return await _context.Tracks
            .OrderBy(t => t.Bpm)
            .ThenBy(t => t.Artist)
            .ThenBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Track>> GetTracksByBpmRangeAsync(float minBpm, float maxBpm)
    {
        return await _context.Tracks
            .Where(t => t.Bpm >= minBpm && t.Bpm <= maxBpm)
            .OrderBy(t => t.Bpm)
            .ToListAsync();
    }
}
