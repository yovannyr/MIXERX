namespace MIXERX.Core.Models.IPC;

public record DeckStatus(
    bool IsPlaying,
    string? CurrentTrack,
    float Position,
    float Duration,
    float Tempo,
    float Volume
);