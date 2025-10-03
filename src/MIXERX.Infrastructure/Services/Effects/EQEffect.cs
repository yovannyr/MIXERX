using MIXERX.Core.Interfaces;

namespace MIXERX.Infrastructure.Services.Effects;

public class EQEffect : IEffect
{
    private float _lowGain = 1.0f;
    private float _midGain = 1.0f;
    private float _highGain = 1.0f;
    
    private readonly BiquadFilter _lowFilter = new(BiquadFilter.FilterType.LowShelf, 250, 48000);
    private readonly BiquadFilter _midFilter = new(BiquadFilter.FilterType.Peaking, 1000, 48000);
    private readonly BiquadFilter _highFilter = new(BiquadFilter.FilterType.HighShelf, 4000, 48000);

    public void Process(Span<float> samples)
    {
        _lowFilter.SetGain(_lowGain);
        _midFilter.SetGain(_midGain);
        _highFilter.SetGain(_highGain);
        
        for (int i = 0; i < samples.Length; i++)
        {
            var sample = samples[i];
            sample = _lowFilter.Process(sample);
            sample = _midFilter.Process(sample);
            sample = _highFilter.Process(sample);
            samples[i] = Math.Clamp(sample, -1.0f, 1.0f);
        }
    }

    public void SetParameter(string name, float value)
    {
        switch (name.ToLower())
        {
            case "low":
                _lowGain = Math.Clamp(value, 0.0f, 2.0f);
                break;
            case "mid":
                _midGain = Math.Clamp(value, 0.0f, 2.0f);
                break;
            case "high":
                _highGain = Math.Clamp(value, 0.0f, 2.0f);
                break;
        }
    }

    public void Reset()
    {
        _lowGain = _midGain = _highGain = 1.0f;
        _lowFilter.Reset();
        _midFilter.Reset();
        _highFilter.Reset();
    }
}

public class BiquadFilter
{
    public enum FilterType { LowShelf, Peaking, HighShelf }
    
    private float _b0, _b1, _b2, _a1, _a2;
    private float _z1, _z2;
    private readonly FilterType _type;
    private readonly float _freq;
    private readonly float _sampleRate;

    public BiquadFilter(FilterType type, float freq, float sampleRate)
    {
        _type = type;
        _freq = freq;
        _sampleRate = sampleRate;
        CalculateCoefficients(1.0f);
    }

    public void SetGain(float gain)
    {
        CalculateCoefficients(gain);
    }

    private void CalculateCoefficients(float gain)
    {
        var w0 = 2 * MathF.PI * _freq / _sampleRate;
        var cosW0 = MathF.Cos(w0);
        var sinW0 = MathF.Sin(w0);
        var A = MathF.Sqrt(gain);
        var alpha = sinW0 / 2;

        switch (_type)
        {
            case FilterType.LowShelf:
                _b0 = A * ((A + 1) - (A - 1) * cosW0 + 2 * MathF.Sqrt(A) * alpha);
                _b1 = 2 * A * ((A - 1) - (A + 1) * cosW0);
                _b2 = A * ((A + 1) - (A - 1) * cosW0 - 2 * MathF.Sqrt(A) * alpha);
                var a0 = (A + 1) + (A - 1) * cosW0 + 2 * MathF.Sqrt(A) * alpha;
                _a1 = -2 * ((A - 1) + (A + 1) * cosW0);
                _a2 = (A + 1) + (A - 1) * cosW0 - 2 * MathF.Sqrt(A) * alpha;
                _b0 /= a0; _b1 /= a0; _b2 /= a0; _a1 /= a0; _a2 /= a0;
                break;

            case FilterType.HighShelf:
                _b0 = A * ((A + 1) + (A - 1) * cosW0 + 2 * MathF.Sqrt(A) * alpha);
                _b1 = -2 * A * ((A - 1) + (A + 1) * cosW0);
                _b2 = A * ((A + 1) + (A - 1) * cosW0 - 2 * MathF.Sqrt(A) * alpha);
                a0 = (A + 1) - (A - 1) * cosW0 + 2 * MathF.Sqrt(A) * alpha;
                _a1 = 2 * ((A - 1) - (A + 1) * cosW0);
                _a2 = (A + 1) - (A - 1) * cosW0 - 2 * MathF.Sqrt(A) * alpha;
                _b0 /= a0; _b1 /= a0; _b2 /= a0; _a1 /= a0; _a2 /= a0;
                break;

            case FilterType.Peaking:
                _b0 = 1 + alpha * A;
                _b1 = -2 * cosW0;
                _b2 = 1 - alpha * A;
                a0 = 1 + alpha / A;
                _a1 = -2 * cosW0;
                _a2 = 1 - alpha / A;
                _b0 /= a0; _b1 /= a0; _b2 /= a0; _a1 /= a0; _a2 /= a0;
                break;
        }
    }

    public float Process(float input)
    {
        var output = _b0 * input + _z1;
        _z1 = _b1 * input - _a1 * output + _z2;
        _z2 = _b2 * input - _a2 * output;
        return output;
    }

    public void Reset()
    {
        _z1 = _z2 = 0;
    }
}
