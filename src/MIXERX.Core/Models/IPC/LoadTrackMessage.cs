namespace MIXERX.Core.Models.IPC;

public record LoadTrackMessage : IpcMessage
{
    public LoadTrackMessage(int deckId, string filePath) : base(IpcMessageType.LoadTrack, deckId)
    {
        StringParam = filePath;
    }
}