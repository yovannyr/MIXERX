namespace MIXERX.Core.Models.IPC;

public record SetLoopMessage : IpcMessage
{
    public SetLoopMessage(int deckId, int beats) : base(IpcMessageType.SetLoop, deckId)
    {
        Data = new Dictionary<string, object> { ["beats"] = beats };
    }
}