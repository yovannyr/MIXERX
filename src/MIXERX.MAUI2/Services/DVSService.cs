namespace MIXERX.MAUI.Services;


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
