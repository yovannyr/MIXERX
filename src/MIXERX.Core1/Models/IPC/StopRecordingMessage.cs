namespace MIXERX.Core.Models.IPC;

public record StopRecordingMessage : IpcMessage
{
    public StopRecordingMessage() : base(IpcMessageType.StopRecording, 0) { }
}