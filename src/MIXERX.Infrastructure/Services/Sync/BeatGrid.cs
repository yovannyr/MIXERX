namespace MIXERX.Infrastructure.Services.Sync;

public class BeatGrid
{
    private readonly float _bpm;
    private readonly int _sampleRate;
    private readonly long _samplesPerBeat;

    public BeatGrid(float bpm, int sampleRate)
    {
        _bpm = bpm;
        _sampleRate = sampleRate;
        _samplesPerBeat = (long)(sampleRate * 60.0 / bpm);
    }

    public float Bpm => _bpm;
    public long SamplesPerBeat => _samplesPerBeat;

    public float GetBeatPhase(long samplePosition)
    {
        if (_samplesPerBeat == 0) return 0;
        return (float)(samplePosition % _samplesPerBeat) / _samplesPerBeat;
    }

    public long GetNearestBeat(long samplePosition)
    {
        if (_samplesPerBeat == 0) return samplePosition;
        
        var beatNumber = samplePosition / _samplesPerBeat;
        var remainder = samplePosition % _samplesPerBeat;
        
        // Snap to nearest beat
        if (remainder > _samplesPerBeat / 2)
        {
            beatNumber++;
        }
        
        return beatNumber * _samplesPerBeat;
    }

    public long GetNextBeat(long samplePosition)
    {
        if (_samplesPerBeat == 0) return samplePosition;
        
        var beatNumber = samplePosition / _samplesPerBeat + 1;
        return beatNumber * _samplesPerBeat;
    }

    public long GetPreviousBeat(long samplePosition)
    {
        if (_samplesPerBeat == 0) return samplePosition;
        
        var beatNumber = samplePosition / _samplesPerBeat;
        return Math.Max(0, beatNumber * _samplesPerBeat);
    }

    public long GetNextBeatSample(long samplePosition)
    {
        return GetNextBeat(samplePosition);
    }
}
