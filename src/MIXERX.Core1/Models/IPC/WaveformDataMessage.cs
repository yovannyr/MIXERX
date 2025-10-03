namespace MIXERX.Core.Models.IPC;

public record WaveformDataMessage : IpcMessage
{
    public WaveformDataMessage(int deckId, float[] waveformData) : base(IpcMessageType.WaveformData, deckId)
    {
        Data = new Dictionary<string, object> { ["waveform"] = waveformData };
    }
}