namespace MIXERX.Engine.Effects;

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
