namespace MIXERX.Core;

public interface IAudioNode
{
    void Process(Span<float> input, Span<float> output, int sampleCount);
    void SetParameter(string name, float value);
    void Reset();
}

public interface IAudioDriver
{
    bool Initialize(AudioConfig config);
    void Start(AudioCallback callback);
    void Stop();
    AudioDeviceInfo[] GetDevices();
    void Dispose();
}

public delegate void AudioCallback(Span<float> input, Span<float> output);

public record AudioDeviceInfo(string Id, string Name, int MaxChannels, int DefaultSampleRate);
