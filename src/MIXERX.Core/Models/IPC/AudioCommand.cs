namespace MIXERX.Core.Models.IPC;

public record AudioCommand(IpcMessageType Type, int DeckId)
{
    public string? StringParam { get; init; }
    public float FloatParam { get; init; }
}