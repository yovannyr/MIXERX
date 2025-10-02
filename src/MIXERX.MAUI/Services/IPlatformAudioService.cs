namespace MIXERX.MAUI.Services;

public interface IPlatformAudioService
{
    Task<IEnumerable<AudioDevice>> GetOutputDevicesAsync();
    Task<IEnumerable<AudioDevice>> GetInputDevicesAsync();
    Task SetOutputDeviceAsync(string deviceId);
    Task<int> GetOptimalBufferSizeAsync();
    bool SupportsLowLatency { get; }
}

public record AudioDevice(string Id, string Name, int SampleRate, int Channels);
