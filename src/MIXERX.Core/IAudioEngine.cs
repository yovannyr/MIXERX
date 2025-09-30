namespace MIXERX.Core;

public interface IAudioEngine
{
    Task<bool> StartAsync(AudioConfig config);
    Task StopAsync();
    void LoadTrack(int deckId, string filePath);
    void SetTempo(int deckId, double bpm);
    void SetPosition(int deckId, TimeSpan position);
    void Play(int deckId);
    void Pause(int deckId);
    AudioMetrics GetMetrics();
}

public class AudioConfig
{
    public int SampleRate { get; set; } = 48000;
    public int BufferSize { get; set; } = 128;
    public AudioApi PreferredApi { get; set; } = AudioApi.Default;
}

public enum AudioApi
{
    Default,
    Wasapi,
    CoreAudio,
    Asio
}

public struct AudioMetrics
{
    public float CpuUsage { get; set; }
    public int BufferUnderruns { get; set; }
    public long SamplesProcessed { get; set; }
    public double LatencyMs { get; set; }
}
