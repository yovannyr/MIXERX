using System.Text.Json;

namespace MIXERX.Core;

// IPC Message Types
public enum IpcMessageType
{
    LoadTrack,
    Play,
    Pause,
    SetTempo,
    SetPosition,
    SetEffectParameter,
    SetCue,
    SetLoop,
    GetStatus,
    StatusResponse
}

// Base IPC Message
public record IpcMessage(IpcMessageType Type, int DeckId = 0)
{
    public string? StringParam { get; init; }
    public float FloatParam { get; init; }
    public Dictionary<string, object>? Data { get; init; }
}

// Specific Message Types
public record LoadTrackMessage : IpcMessage
{
    public LoadTrackMessage(int deckId, string filePath) : base(IpcMessageType.LoadTrack, deckId)
    {
        StringParam = filePath;
    }
}

public record PlayMessage : IpcMessage
{
    public PlayMessage(int deckId) : base(IpcMessageType.Play, deckId) { }
}

public record PauseMessage : IpcMessage
{
    public PauseMessage(int deckId) : base(IpcMessageType.Pause, deckId) { }
}

public record SetTempoMessage : IpcMessage
{
    public SetTempoMessage(int deckId, float tempo) : base(IpcMessageType.SetTempo, deckId)
    {
        FloatParam = tempo;
    }
}

public record SetPositionMessage : IpcMessage
{
    public SetPositionMessage(int deckId, float position) : base(IpcMessageType.SetPosition, deckId)
    {
        FloatParam = position;
    }
}

public record SetCuePointMessage : IpcMessage
{
    public SetCuePointMessage(int deckId, float position) : base(IpcMessageType.SetCue, deckId)
    {
        FloatParam = position;
    }
}

public record SetEffectParameterMessage : IpcMessage
{
    public SetEffectParameterMessage(int deckId, string effectName, string paramName, float value) 
        : base(IpcMessageType.SetEffectParameter, deckId)
    {
        Data = new Dictionary<string, object>
        {
            ["effectName"] = effectName,
            ["paramName"] = paramName,
            ["value"] = value
        };
    }
}

public record GetStatusMessage : IpcMessage
{
    public GetStatusMessage(int deckId) : base(IpcMessageType.GetStatus, deckId) { }
}

public record StatusResponseMessage : IpcMessage
{
    public StatusResponseMessage(int deckId, DeckStatus status) : base(IpcMessageType.StatusResponse, deckId)
    {
        Data = new Dictionary<string, object> { ["status"] = status };
    }
}

// Deck Status
public record DeckStatus(
    bool IsPlaying,
    string? CurrentTrack,
    float Position,
    float Duration,
    float Tempo,
    float Volume
);

// IPC Message Serializer
public static class IpcSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Serialize(IpcMessage message)
    {
        return JsonSerializer.Serialize(message, Options);
    }

    public static IpcMessage? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<IpcMessage>(json, Options);
        }
        catch
        {
            return null;
        }
    }
}
