namespace MIXERX.MAUI.Services;

public class EngineService : IEngineService
{
    public bool IsConnected { get; private set; }

    public async Task<bool> StartEngineAsync()
    {
        // TODO: Implement engine startup
        await Task.Delay(100);
        IsConnected = true;
        return true;
    }

    public async Task StopEngineAsync()
    {
        // TODO: Implement engine shutdown
        await Task.Delay(100);
        IsConnected = false;
    }
}
