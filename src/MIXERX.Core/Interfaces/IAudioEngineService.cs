namespace MIXERX.MAUI.Services;

public interface IAudioEngineService
{
    Task StartEngineAsync();
    Task StopEngineAsync();
    Task<bool> IsEngineRunningAsync();
    Task SendCommandAsync(string command);
}