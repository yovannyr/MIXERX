namespace MIXERX.Core.Models.IPC;

public record SetEffectParameterMessage : IpcMessage
{
    public SetEffectParameterMessage(int deckId, string effectName, string paramName, float value) 
        : base(IpcMessageType.SetEffectParameter, deckId)
    {
        Data = new Dictionary<string, object>
        {
            ["effectName"] = effectName,
            ["paramName"] = paramName,
            ["value"] = value
        };
    }
}