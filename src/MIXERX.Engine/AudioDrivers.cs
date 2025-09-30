using MIXERX.Core;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace MIXERX.Engine;

// Windows WASAPI Driver
public class WasapiDriver : IAudioDriver
{
    private WasapiOut? _wasapiOut;
    private BufferedWaveProvider? _waveProvider;
    private AudioConfig? _config;

    public bool Initialize(AudioConfig config)
    {
        try
        {
            _config = config;
            _waveProvider = new BufferedWaveProvider(new WaveFormat(config.SampleRate, 2))
            {
                BufferLength = config.BufferSize * 4 * 10,
                DiscardOnBufferOverflow = true
            };

            _wasapiOut = new WasapiOut(AudioClientShareMode.Shared, config.BufferSize);
            _wasapiOut.Init(_waveProvider);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Start(AudioCallback callback)
    {
        _wasapiOut?.Play();
    }

    public void Stop()
    {
        _wasapiOut?.Stop();
    }

    public AudioDeviceInfo[] GetDevices()
    {
        try
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            return devices.Select(d => new AudioDeviceInfo(d.ID, d.FriendlyName, 2, 44100)).ToArray();
        }
        catch
        {
            return Array.Empty<AudioDeviceInfo>();
        }
    }

    public void Dispose()
    {
        _wasapiOut?.Dispose();
        _waveProvider = null;
    }
}

// macOS CoreAudio Driver (Basic Implementation)
public class CoreAudioDriver : IAudioDriver
{
    private Thread? _audioThread;
    private volatile bool _isRunning;
    private AudioConfig? _config;

    public bool Initialize(AudioConfig config)
    {
        try
        {
            _config = config;
            // On macOS, we'd use AudioUnit/CoreAudio APIs here
            // For now, return true if running on macOS
            return OperatingSystem.IsMacOS();
        }
        catch
        {
            return false;
        }
    }

    public void Start(AudioCallback callback)
    {
        if (_isRunning || !OperatingSystem.IsMacOS()) return;
        
        _isRunning = true;
        _audioThread = new Thread(() => AudioThreadProc(callback))
        {
            Name = "CoreAudio",
            IsBackground = true
        };
        _audioThread.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _audioThread?.Join(1000);
    }

    public AudioDeviceInfo[] GetDevices()
    {
        return new[] { new AudioDeviceInfo("default", "Default Audio Device", 2, 44100) };
    }

    public void Dispose()
    {
        Stop();
    }

    private void AudioThreadProc(AudioCallback callback)
    {
        var buffer = new float[_config?.BufferSize ?? 512];
        var sleepTime = (int)(1000.0 * buffer.Length / (_config?.SampleRate ?? 44100));
        
        while (_isRunning)
        {
            Array.Clear(buffer);
            callback(buffer.AsSpan(), buffer.AsSpan());
            // On real macOS, this would output to CoreAudio
            Thread.Sleep(sleepTime);
        }
    }
}

// Mock Driver for Testing
public class MockAudioDriver : IAudioDriver
{
    private AudioConfig? _config;
    private AudioCallback? _callback;
    private bool _initialized;

    public bool Initialize(AudioConfig config)
    {
        _config = config;
        _initialized = true;
        return true;
    }

    public void Start(AudioCallback callback)
    {
        if (!_initialized) return;
        _callback = callback;
    }

    public void Stop()
    {
        _callback = null;
    }

    public AudioDeviceInfo[] GetDevices()
    {
        return new[] { new AudioDeviceInfo("mock", "Mock Device", 2, 44100) };
    }

    public void Dispose()
    {
        Stop();
    }
}
