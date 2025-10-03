namespace MIXERX.Core.Models.IPC;

public record BpmDetectedMessage : IpcMessage
{
    public BpmDetectedMessage(int deckId, float bpm) : base(IpcMessageType.BpmDetected, deckId)
    {
        FloatParam = bpm;
    }
}