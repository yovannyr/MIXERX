namespace MIXERX.Core.Models.IPC;

public record SetTempoMessage : IpcMessage
{
    public SetTempoMessage(int deckId, float tempo) : base(IpcMessageType.SetTempo, deckId)
    {
        FloatParam = tempo;
    }
}