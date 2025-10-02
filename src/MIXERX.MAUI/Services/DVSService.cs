namespace MIXERX.MAUI.Services;

public enum TimecodeType
{
    Serato,
    Traktor,
    Mixxx
}

public interface IDVSService
{
    bool IsEnabled { get; set; }
    TimecodeType TimecodeType { get; set; }
    float Pitch { get; }
    bool IsScratching { get; }
    void Calibrate();
}

public class DVSService : IDVSService
{
    public bool IsEnabled { get; set; }
    public TimecodeType TimecodeType { get; set; } = TimecodeType.Serato;
    public float Pitch => 1.0f;
    public bool IsScratching => false;
    
    public void Calibrate()
    {
        // TODO: Implement calibration
    }
}
