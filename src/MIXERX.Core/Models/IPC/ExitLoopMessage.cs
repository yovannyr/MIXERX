namespace MIXERX.Core.Models.IPC;

public record ExitLoopMessage : IpcMessage
{
    public ExitLoopMessage(int deckId) : base(IpcMessageType.ExitLoop, deckId) { }
}