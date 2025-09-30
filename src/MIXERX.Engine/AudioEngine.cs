using MIXERX.Core;
using System.Collections.Concurrent;

namespace MIXERX.Engine;

public class AudioEngine : IAudioEngine
{
    private readonly Dictionary<int, Deck> _decks = new();
    private readonly ConcurrentQueue<AudioCommand> _commandQueue = new();
    private IAudioDriver? _audioDriver;
    private Thread? _audioThread;
    private volatile bool _isRunning;

    public AudioEngine()
    {
        // Initialize 4 decks
        for (int i = 1; i <= 4; i++)
        {
            _decks[i] = new Deck(i);
        }
    }

    public async Task<bool> StartAsync(AudioConfig config)
    {
        if (_isRunning) return true;

        // Create platform-specific audio driver
        _audioDriver = OperatingSystem.IsWindows() 
            ? new WasapiDriver(config) 
            : new CoreAudioDriver(config);

        if (!_audioDriver.Initialize())
            return false;

        _isRunning = true;
        
        // Start real-time audio thread
        _audioThread = new Thread(AudioThreadProc)
        {
            Name = "MIXERX Audio Thread",
            Priority = ThreadPriority.Highest,
            IsBackground = false
        };
        _audioThread.Start(config);

        _audioDriver.Start();
        return true;
    }

    public async Task StopAsync()
    {
        _isRunning = false;
        
        _audioDriver?.Stop();
        _audioThread?.Join(1000);
        _audioDriver?.Dispose();
    }

    private void AudioThreadProc(object? configObj)
    {
        var config = (AudioConfig)configObj!;
        var buffer = new float[config.BufferSize * 2]; // Stereo

        while (_isRunning)
        {
            // Process commands
            while (_commandQueue.TryDequeue(out var command))
            {
                ProcessCommand(command);
            }

            // Generate audio
            ProcessAudio(buffer.AsSpan());
            
            // Send to audio driver
            _audioDriver?.ProcessAudio(buffer.AsSpan());

            // Sleep for buffer duration to maintain real-time
            Thread.Sleep(config.BufferSize * 1000 / config.SampleRate);
        }
    }

    private void ProcessAudio(Span<float> output)
    {
        // Clear output buffer
        output.Clear();
        
        // Mix all decks
        var tempBuffer = new float[output.Length];
        var tempSpan = tempBuffer.AsSpan();
        
        foreach (var deck in _decks.Values)
        {
            if (deck.IsPlaying)
            {
                deck.Process(tempSpan);
                
                // Mix into output
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] += tempSpan[i] * 0.5f; // Simple mixing
                }
            }
            
            tempSpan.Clear();
        }
        
        // Apply master volume and limiting
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = Math.Clamp(output[i] * 0.8f, -1.0f, 1.0f);
        }
    }

    private void ProcessCommand(AudioCommand command)
    {
        if (!_decks.TryGetValue(command.DeckId, out var deck))
            return;

        switch (command.Type)
        {
            case IpcMessageType.LoadTrack:
                deck.LoadTrack(command.StringParam!);
                break;
            case IpcMessageType.Play:
                deck.Play();
                break;
            case IpcMessageType.Pause:
                deck.Pause();
                break;
            case IpcMessageType.SetTempo:
                deck.SetTempo(command.FloatParam);
                break;
            case IpcMessageType.SetPosition:
                deck.SetPosition(TimeSpan.FromSeconds(command.FloatParam));
                break;
        }
    }

    public void LoadTrack(int deckId, string filePath)
    {
        _commandQueue.Enqueue(new AudioCommand(IpcMessageType.LoadTrack, deckId) { StringParam = filePath });
    }

    public void SetTempo(int deckId, double bpm)
    {
        _commandQueue.Enqueue(new AudioCommand(IpcMessageType.SetTempo, deckId) { FloatParam = (float)bpm });
    }

    public void SetPosition(int deckId, TimeSpan position)
    {
        _commandQueue.Enqueue(new AudioCommand(IpcMessageType.SetPosition, deckId) { FloatParam = (float)position.TotalSeconds });
    }

    public void Play(int deckId)
    {
        _commandQueue.Enqueue(new AudioCommand(IpcMessageType.Play, deckId));
    }

    public void Pause(int deckId)
    {
        _commandQueue.Enqueue(new AudioCommand(IpcMessageType.Pause, deckId));
    }

    public AudioMetrics GetMetrics()
    {
        return new AudioMetrics
        {
            LatencyMs = 10.0f, // Estimated
            SamplesProcessed = 0,
            CpuUsage = 0.0f
        };
    }
}

public record AudioCommand(IpcMessageType Type, int DeckId)
{
    public string? StringParam { get; init; }
    public float FloatParam { get; init; }
}
