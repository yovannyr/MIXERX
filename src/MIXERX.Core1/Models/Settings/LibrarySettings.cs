namespace MIXERX.Core.Models.Settings;

public class LibrarySettings
{
    public List<string> Directories { get; set; } = new();
    public bool AutoImport { get; set; } = true;
    public bool ShowItunes { get; set; } = false;
    public bool AnalyzeOnImport { get; set; } = true;
    public string PlayedTrackColor { get; set; } = "blue";
    public int TextSize { get; set; } = 12;
    public bool ProtectLibrary { get; set; } = false;
    public bool CenterOnSelected { get; set; } = false;
}