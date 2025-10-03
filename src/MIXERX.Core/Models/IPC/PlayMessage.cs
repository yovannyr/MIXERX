namespace MIXERX.Core.Models.IPC;

public record PlayMessage : IpcMessage
{
    public PlayMessage(int deckId) : base(IpcMessageType.Play, deckId) { }
}