namespace MIXERX.Core.Models.IPC;

public record PauseMessage : IpcMessage
{
    public PauseMessage(int deckId) : base(IpcMessageType.Pause, deckId) { }
}