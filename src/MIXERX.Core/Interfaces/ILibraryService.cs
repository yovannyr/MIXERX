namespace MIXERX.Core.Interfaces;

public interface ILibraryService
{
    Task<IEnumerable<Track>> SearchAsync(string query);
    Task<Track> AnalyzeTrackAsync(string filePath);
    Task ImportDirectoryAsync(string path);
    Task<IEnumerable<Track>> GetAllTracksAsync();
}