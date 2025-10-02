namespace MIXERX.MAUI.Services;

public interface IKeyDetectionService
{
    Task<string?> DetectKeyAsync(string filePath);
    string GetHarmonicKey(string musicalKey);
    bool AreKeysCompatible(string key1, string key2);
}

public class KeyDetectionService : IKeyDetectionService
{
    private readonly Dictionary<string, string> _camelotWheel = new()
    {
        {"C", "8B"}, {"Am", "8A"}, {"G", "9B"}, {"Em", "9A"},
        {"D", "10B"}, {"Bm", "10A"}, {"A", "11B"}, {"F#m", "11A"},
        {"E", "12B"}, {"C#m", "12A"}, {"B", "1B"}, {"G#m", "1A"},
        {"F#", "2B"}, {"D#m", "2A"}, {"Db", "3B"}, {"Bbm", "3A"},
        {"Ab", "4B"}, {"Fm", "4A"}, {"Eb", "5B"}, {"Cm", "5A"},
        {"Bb", "6B"}, {"Gm", "6A"}, {"F", "7B"}, {"Dm", "7A"}
    };

    public async Task<string?> DetectKeyAsync(string filePath)
    {
        // TODO: Implement key detection algorithm
        await Task.Delay(100);
        return "Am";
    }

    public string GetHarmonicKey(string musicalKey)
    {
        return _camelotWheel.TryGetValue(musicalKey, out var camelot) ? camelot : "?";
    }

    public bool AreKeysCompatible(string key1, string key2)
    {
        var c1 = GetHarmonicKey(key1);
        var c2 = GetHarmonicKey(key2);
        
        if (c1 == "?" || c2 == "?") return false;
        
        // Same key, +1, -1, or energy boost/drop
        return c1 == c2 || 
               Math.Abs(int.Parse(c1[..^1]) - int.Parse(c2[..^1])) <= 1 ||
               (c1[^1] != c2[^1] && c1[..^1] == c2[..^1]);
    }
}
