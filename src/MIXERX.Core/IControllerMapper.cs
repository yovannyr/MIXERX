namespace MIXERX.Core;

public interface IControllerMapper
{
    void LoadMapping(string mappingScript);
    void ProcessMidiMessage(MidiMessage message);
    void SendFeedback(string controlId, object value);
    event Action<string, object>? FeedbackRequested;
}

public record MidiMessage(byte Status, byte Data1, byte Data2)
{
    public bool IsNoteOn(byte note) => (Status & 0xF0) == 0x90 && Data1 == note && Data2 > 0;
    public bool IsNoteOff(byte note) => (Status & 0xF0) == 0x80 && Data1 == note;
    public bool IsCC(byte controller) => (Status & 0xF0) == 0xB0 && Data1 == controller;
    public float Value => Data2 / 127.0f;
}

public class ControllerMapping
{
    public string Name { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ScriptContent { get; set; } = string.Empty;
}
