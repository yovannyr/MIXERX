namespace MIXERX.Core.Models.IPC;

public record SetSyncMessage : IpcMessage
{
    public SetSyncMessage(int deckId, bool enabled) : base(IpcMessageType.SetSync, deckId)
    {
        Data = new Dictionary<string, object> { ["enabled"] = enabled };
    }
}