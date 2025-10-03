using MIXERX.Core.Models.IPC;

namespace MIXERX.Core.Interfaces;

public interface IEngineService
{
    Task<bool> StartEngineAsync();
    Task StopEngineAsync();
    Task LoadTrackAsync(int deckId, string filePath);
    Task PlayAsync(int deckId);
    Task PauseAsync(int deckId);
    Task SetTempoAsync(int deckId, float tempo);
    Task SetPositionAsync(int deckId, float position);
    Task SetCuePointAsync(int deckId, float position);
    Task SetEffectParameterAsync(int deckId, string effectName, string paramName, float value);
    Task<DeckStatus?> GetStatusAsync(int deckId);
    Task StartRecordingAsync(string filePath);
    Task StopRecordingAsync();
    Task SetAutoLoopAsync(int deckId, int beats);
    Task ExitLoopAsync(int deckId);
    Task SetSyncAsync(int deckId, bool enabled);
    bool IsConnected { get; }
    event Action<int, float[]>? WaveformDataReceived;
    event Action<int, bool, int, float>? LoopStatusReceived;
    event Action<int, float>? BpmDetected;
}
