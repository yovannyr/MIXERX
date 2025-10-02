using MIXERX.Core;
using MIXERX.Engine.Audio;
using MIXERX.Engine.Sync;
using MIXERX.Engine.Mixer;
using System.Collections.Concurrent;

namespace MIXERX.Engine;

public class AudioEngine : IAudioEngine
{
    private readonly Dictionary<int, Deck> _decks = new();
    private readonly ConcurrentQueue<AudioCommand> _commandQueue = new();
    private readonly SyncEngine _syncEngine = new();
    private readonly CrossfaderEngine _crossfader = new();
    private IAudioDriver? _audioDriver;
    private Thread? _audioThread;
    private LockFreeAudioBuffer? _masterBuffer;
    private IpcServer? _ipcServer;
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

        // Create platform-specific audio driver with fallback
        try 
        {
            _audioDriver = OperatingSystem.IsWindows() 
                ? new WasapiDriver() 
                : new CoreAudioDriver();
                
            if (!_audioDriver.Initialize(config))
            {
                // Fallback to mock driver if real driver fails
                System.Diagnostics.Debug.WriteLine("Real audio driver failed, using mock driver");
                _audioDriver = new MockAudioDriver();
            }
        }
        catch
        {
            // Fallback to mock driver on any exception
            System.Diagnostics.Debug.WriteLine("Audio driver exception, using mock driver");
            _audioDriver = new MockAudioDriver();
        }

        if (!_audioDriver.Initialize(config))
            return false;

        // Initialize audio buffer
        _masterBuffer = new LockFreeAudioBuffer(config.BufferSize * 4);

        // Start IPC server
        _ipcServer = new IpcServer(this);
        _ipcServer.Start();

        _isRunning = true;
        
        // Start real-time audio thread
        _audioThread = new Thread(AudioThreadProc)
        {
            Name = "MIXERX Audio Thread",
            Priority = ThreadPriority.Highest,
            IsBackground = false
        };
        _audioThread.Start(config);

        _audioDriver.Start(ProcessAudioCallback);
        return true;
    }

    public async Task StopAsync()
    {
        _isRunning = false;
        
        _audioDriver?.Stop();
        _ipcServer?.Dispose();
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

            // Sleep for buffer duration to maintain real-time
            Thread.Sleep(config.BufferSize * 1000 / config.SampleRate);
        }
    }

    private void ProcessAudioCallback(Span<float> input, Span<float> output)
    {
        if (_masterBuffer == null) return;

        // Mix all decks with crossfader
        var mixBuffer = new float[output.Length];
        foreach (var deck in _decks.Values)
        {
            if (deck.IsPlaying)
            {
                var deckBuffer = new float[output.Length];
                deck.GetAudioSamples(deckBuffer, output.Length);
                
                // Apply crossfader volume
                var crossfaderVolume = _crossfader.GetDeckVolume(deck.DeckId);
                
                for (int i = 0; i < output.Length; i++)
                {
                    mixBuffer[i] += deckBuffer[i] * crossfaderVolume;
                }
            }
        }

        // Write to master buffer and read to output
        _masterBuffer.Write(mixBuffer);
        _masterBuffer.Read(output);
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

    public void SetEffectParameter(int deckId, string effectName, string paramName, float value)
    {
        if (_decks.TryGetValue(deckId, out var deck))
        {
            deck.SetEffectParameter(effectName, paramName, value);
        }
    }

    public void SetCrossfader(float position)
    {
        _crossfader.Position = position;
    }

    public void SetCrossfaderCurve(CrossfaderCurve curve)
    {
        _crossfader.Curve = curve;
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
