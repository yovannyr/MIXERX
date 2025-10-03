using System.Text.Json;
using MIXERX.Core.Models.Settings;

namespace MIXERX.MAUI.Services;

public class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MIXERX"
    );
    
    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                var defaultSettings = new AppSettings();
                SetDefaultRecordingDirectory(defaultSettings);
                Save(defaultSettings);
                return defaultSettings;
            }

            var json = File.ReadAllText(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            SetDefaultRecordingDirectory(settings);
            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    public static void Reset()
    {
        var defaultSettings = new AppSettings();
        SetDefaultRecordingDirectory(defaultSettings);
        Save(defaultSettings);
    }

    private static void SetDefaultRecordingDirectory(AppSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Recording.Directory))
        {
            settings.Recording.Directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                "MIXERX"
            );
        }
    }

    public static string GetSettingsDirectory() => SettingsDirectory;
    public static string GetSettingsFilePath() => SettingsFilePath;
}
