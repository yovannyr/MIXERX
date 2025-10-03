using MIXERX.Core;
using MIXERX.Core.Interfaces;

namespace MIXERX.Engine.Audio;

public class MockAudioDriver : IAudioDriver
{
    private Thread? _audioThread;
    private volatile bool _isRunning;
    private AudioConfig? _config;

    public bool Initialize(AudioConfig config)
    {
        _config = config;
        return true;
    }

    public void Start(AudioCallback callback)
    {
        if (_isRunning) return;
        
        _isRunning = true;
        _audioThread = new Thread(() => AudioThreadProc(callback))
        {
            Name = "MockAudio",
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
        return new[] { new AudioDeviceInfo("mock-0", "Mock Audio Device", 2, 44100) };
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
            Thread.Sleep(sleepTime);
        }
    }
}
