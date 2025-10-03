namespace MIXERX.MAUI.Services;

public interface IKeyDetectionService
{
    Task<string?> DetectKeyAsync(string filePath);
    string GetHarmonicKey(string musicalKey);
    bool AreKeysCompatible(string key1, string key2);
}