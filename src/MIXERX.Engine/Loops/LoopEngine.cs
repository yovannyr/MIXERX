using MIXERX.Engine.Sync;

namespace MIXERX.Engine.Loops;

public class LoopEngine
{
    private readonly BeatGrid _beatGrid;
    private bool _isLooping;
    private long _loopStartSample;
    private long _loopEndSample;
    private int _loopLengthBeats;
    private long _currentSample;

    public LoopEngine(BeatGrid beatGrid)
    {
        _beatGrid = beatGrid;
    }

    public bool IsLooping => _isLooping;
    public int LoopLengthBeats => _loopLengthBeats;
    public float LoopProgress => _isLooping ? 
        (float)(_currentSample - _loopStartSample) / (_loopEndSample - _loopStartSample) : 0;

    // Auto-Loop with beat lengths (1, 2, 4, 8, 16, 32 beats)
    public void SetAutoLoop(int beats, long currentSample)
    {
        if (beats <= 0) return;

        _currentSample = currentSample;
        _loopLengthBeats = beats;

        // Find nearest beat for loop start
        var nearestBeat = _beatGrid.GetNextBeatSample(currentSample);
        _loopStartSample = nearestBeat;

        // Calculate loop end based on beat length
        var samplesPerBeat = _beatGrid.GetSamplesPerBeat();
        _loopEndSample = _loopStartSample + (samplesPerBeat * beats);

        _isLooping = true;
    }

    // Manual loop in/out points
    public void SetLoopIn(long samplePosition)
    {
        _loopStartSample = samplePosition;
        if (_isLooping && _loopEndSample <= _loopStartSample)
        {
            _isLooping = false;
        }
    }

    public void SetLoopOut(long samplePosition)
    {
        if (samplePosition > _loopStartSample)
        {
            _loopEndSample = samplePosition;
            _isLooping = true;
        }
    }

    // Process audio sample position for looping
    public long ProcessSamplePosition(long inputSample)
    {
        _currentSample = inputSample;

        if (!_isLooping)
            return inputSample;

        // Check if we've passed the loop end
        if (inputSample >= _loopEndSample)
        {
            // Jump back to loop start
            var overshoot = inputSample - _loopEndSample;
            var loopLength = _loopEndSample - _loopStartSample;
            
            // Handle multiple loop cycles in one call
            if (overshoot >= loopLength)
            {
                overshoot %= loopLength;
            }
            
            return _loopStartSample + overshoot;
        }

        return inputSample;
    }

    // Exit loop
    public void ExitLoop()
    {
        _isLooping = false;
    }

    // Reloop (re-enter last loop)
    public void Reloop()
    {
        if (_loopStartSample < _loopEndSample)
        {
            _isLooping = true;
        }
    }

    // Loop roll (temporary loop that exits automatically)
    public void StartLoopRoll(int beats, long currentSample)
    {
        SetAutoLoop(beats, currentSample);
        // Loop roll would be handled by a timer in the calling code
    }

    // Halve loop length
    public void HalveLoop()
    {
        if (_isLooping && _loopLengthBeats > 1)
        {
            var newLength = _loopLengthBeats / 2;
            var samplesPerBeat = _beatGrid.GetSamplesPerBeat();
            _loopEndSample = _loopStartSample + (samplesPerBeat * newLength);
            _loopLengthBeats = newLength;

            // If current position is beyond new end, jump to start
            if (_currentSample >= _loopEndSample)
            {
                _currentSample = _loopStartSample;
            }
        }
    }

    // Double loop length
    public void DoubleLoop()
    {
        if (_isLooping && _loopLengthBeats < 32)
        {
            var newLength = _loopLengthBeats * 2;
            var samplesPerBeat = _beatGrid.GetSamplesPerBeat();
            _loopEndSample = _loopStartSample + (samplesPerBeat * newLength);
            _loopLengthBeats = newLength;
        }
    }

    // Get loop info for UI display
    public LoopInfo GetLoopInfo()
    {
        return new LoopInfo
        {
            IsActive = _isLooping,
            LengthBeats = _loopLengthBeats,
            Progress = LoopProgress,
            StartSample = _loopStartSample,
            EndSample = _loopEndSample
        };
    }
}

public record LoopInfo
{
    public bool IsActive { get; init; }
    public int LengthBeats { get; init; }
    public float Progress { get; init; }
    public long StartSample { get; init; }
    public long EndSample { get; init; }
}

// Extension for BeatGrid
public static class BeatGridExtensions
{
    public static long GetSamplesPerBeat(this BeatGrid beatGrid)
    {
        // Calculate samples per beat based on BPM and sample rate
        // This would need access to BeatGrid internals
        return 48000 * 60 / 120; // Placeholder: 120 BPM at 48kHz
    }
}
