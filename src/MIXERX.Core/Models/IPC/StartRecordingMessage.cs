namespace MIXERX.Core.Models.IPC;

public record StartRecordingMessage : IpcMessage
{
    public StartRecordingMessage(string filePath) : base(IpcMessageType.StartRecording, 0)
    {
        StringParam = filePath;
    }
}