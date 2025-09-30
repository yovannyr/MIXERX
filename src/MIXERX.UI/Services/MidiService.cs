using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Common;
using MIXERX.Core;

namespace MIXERX.UI.Services;

public interface IMidiService
{
    Task<IEnumerable<MidiDevice>> GetDevicesAsync();
    Task ConnectAsync(string deviceId);
    Task DisconnectAsync();
    event Action<MidiMessage>? MessageReceived;
    void SendMessage(MidiMessage message);
}

public class MidiService : IMidiService, IDisposable
{
    private InputDevice? _inputDevice;
    private OutputDevice? _outputDevice;

    public event Action<MidiMessage>? MessageReceived;

    public Task<IEnumerable<MidiDevice>> GetDevicesAsync()
    {
        var devices = new List<MidiDevice>();
        
        foreach (var device in InputDevice.GetAll())
        {
            devices.Add(new MidiDevice(device.Name, device.Name, MidiDeviceType.Input));
        }
        
        foreach (var device in OutputDevice.GetAll())
        {
            devices.Add(new MidiDevice(device.Name, device.Name, MidiDeviceType.Output));
        }
        
    return Task.FromResult<IEnumerable<MidiDevice>>(devices);
    }

    public Task ConnectAsync(string deviceId)
    {
        try
        {
            // Connect input device
            var inputDevices = InputDevice.GetAll();
            var inputDevice = inputDevices.FirstOrDefault(d => d.Name == deviceId);
            
            if (inputDevice != null)
            {
                _inputDevice = inputDevice;
                _inputDevice.EventReceived += OnMidiEventReceived;
                _inputDevice.StartEventsListening();
            }

            // Connect output device  
            var outputDevices = OutputDevice.GetAll();
            var outputDevice = outputDevices.FirstOrDefault(d => d.Name == deviceId);
            
            if (outputDevice != null)
            {
                _outputDevice = outputDevice;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MIDI connection error: {ex.Message}");
        }
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        _inputDevice?.StopEventsListening();
        _inputDevice?.Dispose();
        _outputDevice?.Dispose();
        
        _inputDevice = null;
        _outputDevice = null;
        return Task.CompletedTask;
    }

    public void SendMessage(MidiMessage message)
    {
        try
        {
            var midiEvent = new NoteOnEvent((SevenBitNumber)message.Data1, (SevenBitNumber)message.Data2);
            _outputDevice?.SendEvent(midiEvent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MIDI send error: {ex.Message}");
        }
    }

    private void OnMidiEventReceived(object? sender, MidiEventReceivedEventArgs e)
    {
        if (e.Event is MidiEvent midiEvent)
        {
            var message = ConvertToMidiMessage(midiEvent);
            MessageReceived?.Invoke(message);
        }
    }

    private static MidiMessage ConvertToMidiMessage(MidiEvent midiEvent)
    {
        return midiEvent switch
        {
            NoteOnEvent noteOn => new MidiMessage((byte)(0x90 | noteOn.Channel), (byte)noteOn.NoteNumber, (byte)noteOn.Velocity),
            NoteOffEvent noteOff => new MidiMessage((byte)(0x80 | noteOff.Channel), (byte)noteOff.NoteNumber, (byte)noteOff.Velocity),
            ControlChangeEvent cc => new MidiMessage((byte)(0xB0 | cc.Channel), (byte)cc.ControlNumber, (byte)cc.ControlValue),
            _ => new MidiMessage(0, 0, 0)
        };
    }

    public void Dispose()
    {
        _ = DisconnectAsync();
    }
}

public record MidiDevice(string Id, string Name, MidiDeviceType Type);

public enum MidiDeviceType
{
    Input,
    Output
}
