namespace MIXERX.Core.Models.IPC;

// Base IPC Message
public record IpcMessage(IpcMessageType Type, int DeckId = 0)
{
    public string? StringParam { get; init; }
    public float FloatParam { get; init; }
    public Dictionary<string, object>? Data { get; init; }
}
