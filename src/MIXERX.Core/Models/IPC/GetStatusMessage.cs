namespace MIXERX.Core.Models.IPC;

public record GetStatusMessage : IpcMessage
{
    public GetStatusMessage(int deckId) : base(IpcMessageType.GetStatus, deckId) { }
}