namespace MIXERX.Core.Models.IPC;

public record LoopStatusMessage : IpcMessage
{
    public LoopStatusMessage(int deckId, bool isLooping, int lengthBeats, float progress) 
        : base(IpcMessageType.LoopStatus, deckId)
    {
        Data = new Dictionary<string, object>
        {
            ["isLooping"] = isLooping,
            ["lengthBeats"] = lengthBeats,
            ["progress"] = progress
        };
    }
}