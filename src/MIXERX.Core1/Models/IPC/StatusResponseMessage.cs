namespace MIXERX.Core.Models.IPC;

public record StatusResponseMessage : IpcMessage
{
    public StatusResponseMessage(int deckId, DeckStatus status) : base(IpcMessageType.StatusResponse, deckId)
    {
        Data = new Dictionary<string, object> { ["status"] = status };
    }
}