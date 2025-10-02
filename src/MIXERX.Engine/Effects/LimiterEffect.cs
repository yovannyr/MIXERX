namespace MIXERX.Engine.Effects;

public class LimiterEffect : IEffect
{
    private float _threshold = 0.95f;
    private float _gainReduction = 1.0f;
    private const float AttackCoeff = 0.999f;
    private const float ReleaseCoeff = 0.9999f;

    public void Process(Span<float> samples)
    {
        for (int i = 0; i < samples.Length; i++)
        {
            var input = samples[i];
            var absInput = MathF.Abs(input);
            
            // Peak detection
            if (absInput > _threshold)
            {
                var targetGain = _threshold / absInput;
                _gainReduction = MathF.Min(_gainReduction, targetGain);
            }
            
            // Apply gain reduction
            samples[i] = input * _gainReduction;
            
            // Release (smooth recovery)
            _gainReduction += (1.0f - _gainReduction) * (1.0f - ReleaseCoeff);
            _gainReduction = MathF.Min(_gainReduction, 1.0f);
        }
    }

    public void SetParameter(string name, float value)
    {
        if (name.ToLower() == "threshold")
        {
            _threshold = Math.Clamp(value, 0.1f, 1.0f);
        }
    }

    public void Reset()
    {
        _threshold = 0.95f;
        _gainReduction = 1.0f;
    }
}
