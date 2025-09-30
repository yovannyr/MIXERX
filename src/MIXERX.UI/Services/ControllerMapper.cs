using Jint;
using MIXERX.Core;

namespace MIXERX.UI.Services;

public class ControllerMapper : IControllerMapper
{
    private Jint.Engine? _jsEngine;
    private readonly Dictionary<int, IDeckController> _decks = new();
    
    public event Action<string, object>? FeedbackRequested;

    public ControllerMapper()
    {
        // Initialize deck controllers
        for (int i = 1; i <= 4; i++)
        {
            _decks[i] = new DeckController(i);
        }
    }

    public void LoadMapping(string mappingScript)
    {
        _jsEngine = new Jint.Engine(options =>
        {
            options.LimitRecursion(100);
            options.TimeoutInterval(TimeSpan.FromMilliseconds(100));
        });

        // Expose API to JavaScript
        _jsEngine.SetValue("getDeck", new Func<int, IDeckController>(GetDeck));
        _jsEngine.SetValue("sendMidi", new Action<byte, byte, byte>(SendMidi));

        try
        {
            _jsEngine.Execute(mappingScript);
        }
        catch (Exception ex)
        {
            // Log mapping script errors for debugging
            System.Diagnostics.Debug.WriteLine($"Mapping script error: {ex.Message}");
        }
    }

    public void ProcessMidiMessage(MidiMessage message)
    {
        try
        {
            _jsEngine?.Invoke("onMidiMessage", message);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MIDI processing error: {ex.Message}");
        }
    }

    public void SendFeedback(string controlId, object value)
    {
        FeedbackRequested?.Invoke(controlId, value);
    }

    private IDeckController GetDeck(int deckId)
    {
        return _decks.GetValueOrDefault(deckId, _decks[1]);
    }

    private void SendMidi(byte status, byte data1, byte data2)
    {
        // Send MIDI feedback to controller hardware
        FeedbackRequested?.Invoke("midi", new MidiMessage(status, data1, data2));
    }
}

public interface IDeckController
{
    void Play();
    void Pause();
    void PlayPause();
    void SetTempo(double tempo);
    void SetPosition(double position);
    void Cue();
    void Sync();
}

public class DeckController : IDeckController
{
    private readonly int _deckId;

    public DeckController(int deckId)
    {
        _deckId = deckId;
    }

    public void Play()
    {
        // Send play command to audio engine via IPC
    }

    public void Pause()
    {
        // Send pause command to audio engine via IPC
    }

    public void PlayPause()
    {
        // Send play/pause toggle command to audio engine via IPC
    }

    public void SetTempo(double tempo)
    {
        // Send tempo change command to audio engine via IPC
    }

    public void SetPosition(double position)
    {
        // Send position change command to audio engine via IPC
    }

    public void Cue()
    {
        // Send cue command to audio engine via IPC
    }

    public void Sync()
    {
        // Send sync command to audio engine via IPC
    }
}
