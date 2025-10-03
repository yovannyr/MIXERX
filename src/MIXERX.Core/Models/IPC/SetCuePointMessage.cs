namespace MIXERX.Core.Models.IPC;

public record SetCuePointMessage : IpcMessage
{
    public SetCuePointMessage(int deckId, float position) : base(IpcMessageType.SetCue, deckId)
    {
        FloatParam = position;
    }
}