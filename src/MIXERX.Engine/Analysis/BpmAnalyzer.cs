namespace MIXERX.Engine.Analysis;

public class BpmAnalyzer
{
    private readonly int _sampleRate;
    private readonly List<float> _energyBuffer = new();
    private readonly List<float> _beatTimes = new();
    private float _lastEnergy;
    private int _sampleCount;

    public BpmAnalyzer(int sampleRate = 48000)
    {
        _sampleRate = sampleRate;
    }

    public float AnalyzeBpm(float[] samples)
    {
        // Simple beat detection algorithm
        var energy = CalculateEnergy(samples);
        _energyBuffer.Add(energy);
        
        // Keep only recent energy values (last 10 seconds)
        var maxBufferSize = _sampleRate * 10 / samples.Length;
        if (_energyBuffer.Count > maxBufferSize)
        {
            _energyBuffer.RemoveAt(0);
        }

        // Detect beats (energy peaks)
        if (IsEnergyPeak(energy))
        {
            var currentTime = _sampleCount / (float)_sampleRate;
            _beatTimes.Add(currentTime);
            
            // Keep only recent beats (last 30 seconds)
            _beatTimes.RemoveAll(t => currentTime - t > 30);
        }

        _lastEnergy = energy;
        _sampleCount += samples.Length;

        return CalculateBpmFromBeats();
    }

    private float CalculateEnergy(float[] samples)
    {
        float energy = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            energy += samples[i] * samples[i];
        }
        return energy / samples.Length;
    }

    private bool IsEnergyPeak(float energy)
    {
        if (_energyBuffer.Count < 10) return false;

        // Check if current energy is significantly higher than recent average
        var recentAverage = _energyBuffer.TakeLast(10).Average();
        var threshold = recentAverage * 1.3f; // 30% above average
        
        return energy > threshold && energy > _lastEnergy;
    }

    private float CalculateBpmFromBeats()
    {
        if (_beatTimes.Count < 4) return 0; // Need at least 4 beats

        // Calculate intervals between beats
        var intervals = new List<float>();
        for (int i = 1; i < _beatTimes.Count; i++)
        {
            intervals.Add(_beatTimes[i] - _beatTimes[i - 1]);
        }

        // Find most common interval (mode)
        var avgInterval = intervals.Where(i => i > 0.3f && i < 2.0f) // Filter reasonable intervals
                                  .DefaultIfEmpty(1.0f)
                                  .Average();

        // Convert interval to BPM
        var bpm = 60.0f / avgInterval;
        
        // Clamp to reasonable BPM range
        return Math.Clamp(bpm, 60, 200);
    }

    public void Reset()
    {
        _energyBuffer.Clear();
        _beatTimes.Clear();
        _lastEnergy = 0;
        _sampleCount = 0;
    }
}

// Key Detection (simplified)
public class KeyAnalyzer
{
    private readonly string[] _keys = 
    {
        "C", "C#", "D", "D#", "E", "F", 
        "F#", "G", "G#", "A", "A#", "B"
    };

    public string AnalyzeKey(float[] samples)
    {
        // Simplified key detection using pitch class profile
        var pitchProfile = new float[12];
        
        // Simple FFT-like analysis (very basic)
        for (int i = 0; i < samples.Length - 1; i++)
        {
            var freq = EstimateFrequency(samples, i);
            if (freq > 80 && freq < 2000) // Focus on musical range
            {
                var pitchClass = FrequencyToPitchClass(freq);
                pitchProfile[pitchClass] += Math.Abs(samples[i]);
            }
        }

        // Find dominant pitch class
        var maxIndex = 0;
        for (int i = 1; i < pitchProfile.Length; i++)
        {
            if (pitchProfile[i] > pitchProfile[maxIndex])
                maxIndex = i;
        }

        return _keys[maxIndex];
    }

    private float EstimateFrequency(float[] samples, int index)
    {
        // Very simple frequency estimation
        if (index >= samples.Length - 1) return 0;
        
        var period = Math.Abs(samples[index + 1] - samples[index]);
        return period > 0 ? 48000.0f / (period * 1000) : 0;
    }

    private int FrequencyToPitchClass(float frequency)
    {
        // Convert frequency to pitch class (0-11)
        var noteNumber = 12 * Math.Log2(frequency / 440.0) + 69; // A4 = 440Hz = note 69
        return ((int)Math.Round(noteNumber) % 12 + 12) % 12;
    }
}
