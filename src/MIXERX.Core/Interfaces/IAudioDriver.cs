namespace MIXERX.Core.Interfaces;

public interface IAudioDriver
{
    bool Initialize(AudioConfig config);
    void Start(AudioCallback callback);
    void Stop();
    AudioDeviceInfo[] GetDevices();
    void Dispose();
}