namespace MIXERX.Core.Models.Settings;

public class PrivacySettings
{
    public bool SendUsageData { get; set; } = true;
    public bool CrashReports { get; set; } = true;
    public bool AnonymousStats { get; set; } = true;
}