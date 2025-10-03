namespace MIXERX.Core.Interfaces;

public interface IDVSService
{
    bool IsEnabled { get; set; }
    TimecodeType TimecodeType { get; set; }
    float Pitch { get; }
    bool IsScratching { get; }
    void Calibrate();
}