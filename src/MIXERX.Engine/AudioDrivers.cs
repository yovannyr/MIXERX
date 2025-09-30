using MIXERX.Core;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace MIXERX.Engine;

// Windows WASAPI Driver
public class WasapiDriver : IAudioDriver
{
    private WasapiOut? _wasapiOut;
    private BufferedWaveProvider? _waveProvider;
    private readonly AudioConfig _config;

    public WasapiDriver(AudioConfig config)
    {
        _config = config;
    }

    public bool Initialize()
    {
        try
        {
            _waveProvider = new BufferedWaveProvider(new WaveFormat(_config.SampleRate, 2))
            {
                BufferLength = _config.BufferSize * 4 * 10, // 10 buffers
                DiscardOnBufferOverflow = true
            };

            _wasapiOut = new WasapiOut(AudioClientShareMode.Shared, _config.BufferSize);
            _wasapiOut.Init(_waveProvider);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Start()
    {
        _wasapiOut?.Play();
    }

    public void Stop()
    {
        _wasapiOut?.Stop();
    }

    public IEnumerable<AudioDevice> EnumerateDevices()
    {
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        
        return devices.Select(d => new AudioDevice(d.ID, d.FriendlyName));
    }

    public void ProcessAudio(Span<float> samples)
    {
        if (_waveProvider == null) return;

        // Convert float samples to byte array
        var bytes = new byte[samples.Length * 4];
        for (int i = 0; i < samples.Length; i++)
        {
            var sample = Math.Clamp(samples[i], -1.0f, 1.0f);
            var intSample = (int)(sample * int.MaxValue);
            BitConverter.GetBytes(intSample).CopyTo(bytes, i * 4);
        }

        _waveProvider.AddSamples(bytes, 0, bytes.Length);
    }

    public void Dispose()
    {
        _wasapiOut?.Dispose();
        _waveProvider = null;
    }
}

// macOS CoreAudio Driver (Stub for cross-platform)
public class CoreAudioDriver : IAudioDriver
{
    private readonly AudioConfig _config;

    public CoreAudioDriver(AudioConfig config)
    {
        _config = config;
    }

    public bool Initialize()
    {
        // NAudio doesn't support CoreAudio directly on macOS
        // This would need platform-specific implementation
        return false;
    }

    public void Start() { }
    public void Stop() { }

    public IEnumerable<AudioDevice> EnumerateDevices()
    {
        return [];
    }

    public void ProcessAudio(Span<float> samples) { }
    public void Dispose() { }
}

public interface IAudioDriver : IDisposable
{
    bool Initialize();
    void Start();
    void Stop();
    IEnumerable<AudioDevice> EnumerateDevices();
    void ProcessAudio(Span<float> samples);
}

public record AudioDevice(string Id, string Name);
