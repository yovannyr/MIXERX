namespace MIXERX.Engine.Effects;

public interface IEffect
{
    void Process(Span<float> samples);
    void SetParameter(string name, float value);
    void Reset();
}

public class EffectChain : IEffect
{
    private readonly List<IEffect> _effects = new();

    public void AddEffect(IEffect effect)
    {
        _effects.Add(effect);
    }

    public void RemoveEffect(IEffect effect)
    {
        _effects.Remove(effect);
    }

    public void Process(Span<float> samples)
    {
        foreach (var effect in _effects)
        {
            effect.Process(samples);
        }
    }

    public void SetParameter(string name, float value)
    {
        // Route to specific effect based on parameter name
        var parts = name.Split('.');
        if (parts.Length == 2)
        {
            var effectName = parts[0];
            var paramName = parts[1];
            
            var effect = _effects.FirstOrDefault(e => e.GetType().Name.StartsWith(effectName));
            effect?.SetParameter(paramName, value);
        }
    }

    public void Reset()
    {
        foreach (var effect in _effects)
        {
            effect.Reset();
        }
    }
}

// 3-Band EQ Effect
public class EQEffect : IEffect
{
    private float _lowGain = 1.0f;
    private float _midGain = 1.0f;
    private float _highGain = 1.0f;
    
    // Simple biquad filter coefficients
    private float _lowState1, _lowState2;
    private float _midState1, _midState2;
    private float _highState1, _highState2;

    public void Process(Span<float> samples)
    {
        for (int i = 0; i < samples.Length; i += 2)
        {
            // Simple 3-band EQ approximation
            var sample = samples[i];
            
            // Low band (< 300Hz)
            var low = ProcessBiquad(sample, 0.1f, ref _lowState1, ref _lowState2) * _lowGain;
            
            // Mid band (300Hz - 3kHz)  
            var mid = ProcessBiquad(sample, 0.5f, ref _midState1, ref _midState2) * _midGain;
            
            // High band (> 3kHz)
            var high = ProcessBiquad(sample, 0.9f, ref _highState1, ref _highState2) * _highGain;
            
            var output = (low + mid + high) * 0.33f;
            
            samples[i] = Math.Clamp(output, -1.0f, 1.0f);
            if (i + 1 < samples.Length)
                samples[i + 1] = samples[i]; // Mono to stereo
        }
    }

    private float ProcessBiquad(float input, float freq, ref float state1, ref float state2)
    {
        // Simple biquad filter approximation
        var output = input * freq + state1 * 0.5f + state2 * 0.25f;
        state2 = state1;
        state1 = input;
        return output;
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
        _lowState1 = _lowState2 = 0;
        _midState1 = _midState2 = 0;
        _highState1 = _highState2 = 0;
    }
}

// Low-Pass Filter Effect
public class FilterEffect : IEffect
{
    private float _cutoff = 1.0f;
    private float _resonance = 0.1f;
    private float _state1, _state2;

    public void Process(Span<float> samples)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            var input = samples[i];
            
            // Simple low-pass filter
            _state1 += _cutoff * (input - _state1 + _resonance * (_state1 - _state2));
            _state2 += _cutoff * (_state1 - _state2);
            
            samples[i] = Math.Clamp(_state2, -1.0f, 1.0f);
        }
    }

    public void SetParameter(string name, float value)
    {
        switch (name.ToLower())
        {
            case "cutoff":
                _cutoff = Math.Clamp(value, 0.01f, 1.0f);
                break;
            case "resonance":
                _resonance = Math.Clamp(value, 0.0f, 0.9f);
                break;
        }
    }

    public void Reset()
    {
        _cutoff = 1.0f;
        _resonance = 0.1f;
        _state1 = _state2 = 0;
    }
}
