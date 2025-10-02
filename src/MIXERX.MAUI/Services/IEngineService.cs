namespace MIXERX.MAUI.Services;

public interface IEngineService
{
    Task<bool> StartEngineAsync();
    Task StopEngineAsync();
    bool IsConnected { get; }
}
