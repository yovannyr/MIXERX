namespace MIXERX.Core.Models.IPC;

public record SetPositionMessage : IpcMessage
{
    public SetPositionMessage(int deckId, float position) : base(IpcMessageType.SetPosition, deckId)
    {
        FloatParam = position;
    }
}