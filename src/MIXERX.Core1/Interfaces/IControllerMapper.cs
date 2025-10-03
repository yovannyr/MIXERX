namespace MIXERX.Core.Interfaces;

public interface IControllerMapper
{
    void LoadMapping(string mappingScript);
    void ProcessMidiMessage(MidiMessage message);
    void SendFeedback(string controlId, object value);
    event Action<string, object>? FeedbackRequested;
}

