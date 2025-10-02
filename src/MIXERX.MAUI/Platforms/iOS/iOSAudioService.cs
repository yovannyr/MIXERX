#if IOS
using MIXERX.MAUI.Services;

namespace MIXERX.MAUI.Platforms.iOS;

public class iOSAudioService : IPlatformAudioService
{
    public bool SupportsLowLatency => true; // AVAudioEngine

    public Task<IEnumerable<AudioDevice>> GetOutputDevicesAsync()
    {
        // TODO: Use AVFoundation.AVAudioSession
        var devices = new List<AudioDevice>
        {
            new AudioDevice("default", "Default Output", 48000, 2)
        };
        return Task.FromResult<IEnumerable<AudioDevice>>(devices);
    }

    public Task<IEnumerable<AudioDevice>> GetInputDevicesAsync()
    {
        // TODO: Use AVFoundation.AVAudioSession
        var devices = new List<AudioDevice>
        {
            new AudioDevice("default", "Default Input", 48000, 2)
        };
        return Task.FromResult<IEnumerable<AudioDevice>>(devices);
    }

    public Task SetOutputDeviceAsync(string deviceId)
    {
        // TODO: Configure AVAudioSession output
        return Task.CompletedTask;
    }

    public Task<int> GetOptimalBufferSizeAsync()
    {
        // AVAudioEngine: 256-512 samples (~5-11ms at 48kHz)
        return Task.FromResult(256);
    }
}
#endif
