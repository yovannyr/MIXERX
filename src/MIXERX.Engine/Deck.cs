using MIXERX.Core;
using MIXERX.Engine.Effects;
using MIXERX.Engine.Analysis;
using MIXERX.Engine.Sync;
using MIXERX.Engine.Loops;
using MIXERX.Engine.Cues;
using MIXERX.Engine.AI;
using MIXERX.Engine.Codecs;
using System.Collections.Concurrent;

namespace MIXERX.Engine;

public class Deck : IAudioNode
{
    private readonly int _deckId;
    private readonly ConcurrentQueue<DeckCommand> _commandQueue = new();
    private IAudioDecoder? _decoder;
    private AudioData? _currentTrack;
    private bool _isPlaying;
    private float _tempo = 1.0f;
    private float _volume = 1.0f;
    private int _position;
    private readonly float[] _readBuffer = new float[8192];
    
    // Advanced features
    private readonly EffectChain _effectChain = new();
    private readonly BpmAnalyzer _bpmAnalyzer = new();
    private readonly KeyAnalyzer _keyAnalyzer = new();
    private readonly HotCueEngine _hotCueEngine = new();
    
    // AI Features
    private readonly NeuralBpmDetector _neuralBpmDetector = new();
    private readonly IntelligentKeyDetector _intelligentKeyDetector = new();
    private readonly HarmonicMixingEngine _harmonicMixingEngine = new();
    
    private BeatGrid? _beatGrid;
    private LoopEngine? _loopEngine;
    private long _samplePosition;
    
    // Track info with AI enhancement
    private float _detectedBpm;
    private string _detectedKey = "";
    private float _aiConfidence;
    private KeyDetectionResult? _keyDetectionResult;

    public Deck(int deckId)
    {
        _deckId = deckId;
        
        // Initialize default effects
        _effectChain.AddEffect(new EQEffect());
        _effectChain.AddEffect(new FilterEffect());
        _effectChain.AddEffect(new ReverbEffect());
        _effectChain.AddEffect(new DelayEffect());
    }

    public bool IsPlaying => _isPlaying;
    public string? CurrentTrack { get; private set; }
    public TimeSpan Position => _decoder?.Duration ?? TimeSpan.Zero;
    public TimeSpan Duration => _decoder?.Duration ?? TimeSpan.Zero;
    public float DetectedBpm => _detectedBpm;
    public string DetectedKey => _detectedKey;
    public float AIConfidence => _aiConfidence;
    public float CurrentPhase => _beatGrid?.GetBeatPhase(_samplePosition) ?? 0;
    public LoopInfo? LoopInfo => _loopEngine?.GetLoopInfo();
    public HotCue[] HotCues => _hotCueEngine.GetAllHotCues();
    public KeyDetectionResult? KeyDetectionResult => _keyDetectionResult;

    // AI-powered analysis methods
    public HarmonicMixingRecommendation GetHarmonicRecommendations(IEnumerable<string> availableKeys)
    {
        return _harmonicMixingEngine.GetHarmonicRecommendations(_detectedKey, availableKeys);
    }

    public string[] GetKeyProgression(int steps = 4)
    {
        return _harmonicMixingEngine.GetKeyProgression(_detectedKey, steps);
    }

    // Hot Cue controls
    public void SetHotCue(int cueNumber, string? label = null)
    {
        _hotCueEngine.SetHotCue(cueNumber, _samplePosition, label);
    }

    public void TriggerHotCue(int cueNumber)
    {
        var position = _hotCueEngine.TriggerHotCue(cueNumber);
        if (position.HasValue)
        {
            _samplePosition = position.Value;
            var timePosition = TimeSpan.FromSeconds((double)position.Value / (_decoder?.SampleRate ?? 48000));
            _decoder?.Seek(timePosition);
        }
    }

    public void DeleteHotCue(int cueNumber)
    {
        _hotCueEngine.DeleteHotCue(cueNumber);
    }

    // Loop controls
    public void SetAutoLoop(int beats)
    {
        _loopEngine?.SetAutoLoop(beats, _samplePosition);
    }

    public void SetLoopIn()
    {
        _loopEngine?.SetLoopIn(_samplePosition);
    }

    public void SetLoopOut()
    {
        _loopEngine?.SetLoopOut(_samplePosition);
    }

    public void ExitLoop()
    {
        _loopEngine?.ExitLoop();
    }

    public void Reloop()
    {
        _loopEngine?.Reloop();
    }

    public void HalveLoop()
    {
        _loopEngine?.HalveLoop();
    }

    public void DoubleLoop()
    {
        _loopEngine?.DoubleLoop();
    }

    public void LoadTrack(string filePath)
    {
        _commandQueue.Enqueue(new DeckCommand(DeckCommandType.LoadTrack, filePath));
    }

    public void Play()
    {
        _commandQueue.Enqueue(new DeckCommand(DeckCommandType.Play));
    }

    public void Pause()
    {
        _commandQueue.Enqueue(new DeckCommand(DeckCommandType.Pause));
    }

    public void SetTempo(float tempo)
    {
        _tempo = Math.Clamp(tempo, 0.5f, 2.0f);
    }

    public void SetVolume(float volume)
    {
        _volume = Math.Clamp(volume, 0.0f, 1.0f);
    }

    public void SetPosition(TimeSpan position)
    {
        var newSamplePosition = (long)(position.TotalSeconds * (_decoder?.SampleRate ?? 48000));
        
        // If looping, don't allow seeking outside loop
        if (_loopEngine?.IsLooping == true)
        {
            var loopInfo = _loopEngine.GetLoopInfo();
            if (newSamplePosition < loopInfo.StartSample || newSamplePosition >= loopInfo.EndSample)
            {
                return; // Ignore seek outside loop
            }
        }
        
        _decoder?.Seek(position);
        _samplePosition = newSamplePosition;
    }

    public void SetEffectParameter(string effectName, string paramName, float value)
    {
        _effectChain.SetParameter($"{effectName}.{paramName}", value);
    }

    public void Process(Span<float> output, int sampleCount)
    {
        // Process commands
        while (_commandQueue.TryDequeue(out var command))
        {
            ProcessCommand(command);
        }

        output.Clear();

        if (!_isPlaying || _decoder == null)
            return;

        // Process loop position
        var actualSamplePosition = _loopEngine?.ProcessSamplePosition(_samplePosition) ?? _samplePosition;
        
        // If loop caused a jump, seek decoder
        if (actualSamplePosition != _samplePosition)
        {
            var seekTime = TimeSpan.FromSeconds(actualSamplePosition / (double)(_decoder.SampleRate));
            _decoder.Seek(seekTime);
            _samplePosition = actualSamplePosition;
        }

        // Read audio samples
        var samplesRead = _decoder.Read(_readBuffer, 0, Math.Min(_readBuffer.Length, sampleCount));
        
        if (samplesRead == 0)
        {
            _isPlaying = false;
            return;
        }

        // Apply tempo stretching (simple pitch-preserving)
        ApplyTempoStretching(_readBuffer, samplesRead);

        // Apply effects
        _effectChain.Process(_readBuffer.AsSpan(0, samplesRead));

        // Apply volume and copy to output
        for (int i = 0; i < Math.Min(samplesRead, output.Length); i++)
        {
            output[i] = _readBuffer[i] * _volume;
        }

        // Update position and analyze
        _samplePosition += samplesRead;
        
        // AI-powered analysis (every 2048 samples for better accuracy)
        if (_samplePosition % 2048 == 0)
        {
            AnalyzeAudioWithAI(_readBuffer, samplesRead);
        }
    }

    private void ApplyTempoStretching(float[] samples, int count)
    {
        if (Math.Abs(_tempo - 1.0f) < 0.01f) return; // No stretching needed

        // Simple tempo stretching by sample rate adjustment
        // In production, use PSOLA or similar algorithm
        var stretchRatio = 1.0f / _tempo;
        
        for (int i = 0; i < count - 1; i++)
        {
            var sourceIndex = i * stretchRatio;
            var index1 = (int)sourceIndex;
            var index2 = Math.Min(index1 + 1, count - 1);
            var fraction = sourceIndex - index1;
            
            // Linear interpolation
            samples[i] = samples[index1] * (1 - fraction) + samples[index2] * fraction;
        }
    }

    private void AnalyzeAudioWithAI(float[] samples, int count)
    {
        var sampleRate = _decoder?.SampleRate ?? 48000;
        
        // AI-powered BPM detection
        var neuralBpm = _neuralBpmDetector.DetectBpm(samples[..count], sampleRate);
        
        // Traditional BPM detection as backup
        var traditionalBpm = _bpmAnalyzer.AnalyzeBpm(samples[..count]);
        
        // Combine results with confidence weighting
        if (neuralBpm > 0)
        {
            _detectedBpm = neuralBpm;
            _aiConfidence = 0.95f; // High confidence for neural detection
            _beatGrid = new BeatGrid(neuralBpm, sampleRate);
            _loopEngine = new LoopEngine(_beatGrid);
        }
        else if (traditionalBpm > 0)
        {
            _detectedBpm = traditionalBpm;
            _aiConfidence = 0.7f; // Lower confidence for traditional
            _beatGrid = new BeatGrid(traditionalBpm, sampleRate);
            _loopEngine = new LoopEngine(_beatGrid);
        }

        // AI-powered key detection (less frequent)
        if (_samplePosition % (sampleRate * 5) == 0) // Every 5 seconds
        {
            _keyDetectionResult = _intelligentKeyDetector.DetectKey(samples[..count], sampleRate);
            _detectedKey = _keyDetectionResult.Key;
            _aiConfidence = Math.Max(_aiConfidence, _keyDetectionResult.Confidence);
        }
    }

    public void GetAudioSamples(float[] buffer, int frames)
    {
        if (!_isPlaying || _currentTrack == null)
        {
            Array.Fill(buffer, 0.0f);
            return;
        }

        // Read from current track samples
        var available = Math.Min(frames, _currentTrack.Samples.Length - _position);
        Array.Copy(_currentTrack.Samples, _position, buffer, 0, available);
        _position += available;
        
        // Apply volume
        for (int i = 0; i < available; i++)
        {
            buffer[i] *= _volume;
        }

        // Fill remaining with silence
        for (int i = available; i < frames; i++)
        {
            buffer[i] = 0.0f;
        }
    }

    public void Process(Span<float> input, Span<float> output, int sampleCount)
    {
        Process(output, sampleCount);
    }

    public void Process(Span<float> output)
    {
        Process(output, output.Length);
    }

    public void SetParameter(string name, float value)
    {
        switch (name.ToLower())
        {
            case "tempo":
                SetTempo(value);
                break;
            case "volume":
                SetVolume(value);
                break;
            default:
                // Try to route to effects
                _effectChain.SetParameter(name, value);
                break;
        }
    }

    public void Reset()
    {
        _isPlaying = false;
        _tempo = 1.0f;
        _volume = 1.0f;
        _effectChain.Reset();
        _bpmAnalyzer.Reset();
        _samplePosition = 0;
        _loopEngine?.ExitLoop();
        _hotCueEngine.ClearAllHotCues();
        _aiConfidence = 0;
        _keyDetectionResult = null;
    }

    private void ProcessCommand(DeckCommand command)
    {
        switch (command.Type)
        {
            case DeckCommandType.LoadTrack:
                LoadTrackInternal(command.StringParam!);
                break;
            case DeckCommandType.Play:
                _isPlaying = true;
                break;
            case DeckCommandType.Pause:
                _isPlaying = false;
                break;
        }
    }

    private void LoadTrackInternal(string filePath)
    {
        try
        {
            _decoder?.Dispose();
            
            // Use FFmpegAudioDecoder for all formats (includes WAV fallback)
            var ffmpegDecoder = new FFmpegAudioDecoder();
            var audioData = ffmpegDecoder.LoadFile(filePath);
            
            if (audioData != null)
            {
                _currentTrack = audioData;
                _decoder = ffmpegDecoder as IAudioDecoder;
            }
            else
            {
                ffmpegDecoder.Dispose();
                _decoder = null;
                CurrentTrack = null;
                return;
            }
            
            CurrentTrack = Path.GetFileNameWithoutExtension(filePath);
            _isPlaying = false;
            _samplePosition = 0;
            _position = 0;
            _bpmAnalyzer.Reset();
            _detectedBpm = 0;
            _detectedKey = "";
            _aiConfidence = 0;
            _keyDetectionResult = null;
            _loopEngine?.ExitLoop();
            
            // Load hot cues for this track
            _hotCueEngine.LoadHotCuesFromTrack(filePath);
        }
        catch
        {
            _decoder?.Dispose();
            _decoder = null;
            _currentTrack = null;
            CurrentTrack = null;
        }
    }

    public void Dispose()
    {
        _decoder?.Dispose();
    }
}

public enum DeckCommandType
{
    LoadTrack,
    Play,
    Pause
}

public record DeckCommand(DeckCommandType Type, string? StringParam = null);
