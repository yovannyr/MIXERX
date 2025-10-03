
namespace MIXERX.Core.Interfaces;

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
