using MIXERX.Core;
using MIXERX.Core.Interfaces;

namespace MIXERX.Infrastructure.Services.Effects;

public class ReverbEffect : IEffect
{
    private readonly float[] _delayBuffer;
    private int _delayIndex;
    private float _wetLevel = 0.0f;
    private float _feedback = 0.5f;
    private readonly int _delayLength;

    public string Name => "Reverb";
    public bool IsEnabled { get; set; } = true;

    public ReverbEffect(int sampleRate = 48000)
    {
        _delayLength = (int)(sampleRate * 0.1); // 100ms delay
        _delayBuffer = new float[_delayLength];
    }

    public void Process(Span<float> buffer)
    {
        if (!IsEnabled)
        {
            return;
        }

        for (int i = 0; i < buffer.Length; i++)
        {
            // Get delayed sample
            var delayedSample = _delayBuffer[_delayIndex];
            
            // Mix input with feedback
            var inputSample = buffer[i] + (delayedSample * _feedback);
            
            // Store in delay buffer
            _delayBuffer[_delayIndex] = inputSample;
            
            // Mix dry and wet signals
            buffer[i] = buffer[i] * (1.0f - _wetLevel) + delayedSample * _wetLevel;
            
            // Advance delay index
            _delayIndex = (_delayIndex + 1) % _delayLength;
        }
    }

    public void SetParameter(string name, float value)
    {
        switch (name.ToLower())
        {
            case "wet":
            case "mix":
                _wetLevel = Math.Clamp(value, 0.0f, 1.0f);
                break;
            case "feedback":
                _feedback = Math.Clamp(value, 0.0f, 0.95f);
                break;
        }
    }

    public void Reset()
    {
        Array.Clear(_delayBuffer);
        _delayIndex = 0;
        _wetLevel = 0.0f;
        _feedback = 0.5f;
    }
}
