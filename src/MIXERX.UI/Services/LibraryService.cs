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
            Duration = TimeSpan.Zero, // Will be extracted from audio file metadata
            LastModified = fileInfo.LastWriteTime
        };

        // Audio metadata extraction will be implemented with TagLib# or similar
        // BPM and Key analysis will be added in future iterations

        _context.Tracks.Add(track);
        await _context.SaveChangesAsync();
        
        return track;
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
        return await _context.Tracks.ToListAsync();
    }
}
