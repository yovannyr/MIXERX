namespace MIXERX.MAUI.Services;

public interface IMidiService
{
    Task<IEnumerable<MidiDevice>> GetDevicesAsync();
    Task ConnectAsync(string deviceId);
    Task DisconnectAsync();
    event Action<MidiMessage>? MessageReceived;
    void SendMessage(MidiMessage message);
}