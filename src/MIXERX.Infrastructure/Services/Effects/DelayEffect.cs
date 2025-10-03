using MIXERX.Core;
using MIXERX.Core.Interfaces;

namespace MIXERX.Infrastructure.Services.Effects;

public class DelayEffect : IEffect
{
    private readonly float[] _delayBuffer;
    private int _writeIndex;
    private int _readIndex;
    private float _wetLevel = 0.0f;
    private float _feedback = 0.3f;
    private float _delayTime = 0.25f; // 1/4 note delay
    private readonly int _maxDelayLength;
    private readonly int _sampleRate;

    public string Name => "Delay";
    public bool IsEnabled { get; set; } = true;

    public DelayEffect(int sampleRate = 48000)
    {
        _sampleRate = sampleRate;
        _maxDelayLength = sampleRate; // 1 second max delay
        _delayBuffer = new float[_maxDelayLength];
        UpdateDelayLength();
    }

    public void Process(Span<float> buffer)
    {
        if (!IsEnabled)
        {
            return;
        }

        for (int i = 0; i < buffer.Length; i++)
        {
            // Read delayed sample
            var delayedSample = _delayBuffer[_readIndex];
            
            // Write input + feedback to delay buffer
            _delayBuffer[_writeIndex] = buffer[i] + (delayedSample * _feedback);
            
            // Mix dry and wet signals
            buffer[i] = buffer[i] * (1.0f - _wetLevel) + delayedSample * _wetLevel;
            
            // Advance indices
            _writeIndex = (_writeIndex + 1) % _maxDelayLength;
            _readIndex = (_readIndex + 1) % _maxDelayLength;
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
            case "time":
                _delayTime = Math.Clamp(value, 0.01f, 1.0f);
                UpdateDelayLength();
                break;
        }
    }

    private void UpdateDelayLength()
    {
        var delayLength = (int)(_sampleRate * _delayTime);
        _readIndex = (_writeIndex - delayLength + _maxDelayLength) % _maxDelayLength;
    }

    public void Reset()
    {
        Array.Clear(_delayBuffer);
        _writeIndex = 0;
        _readIndex = 0;
        _wetLevel = 0.0f;
        _feedback = 0.3f;
        _delayTime = 0.25f;
        UpdateDelayLength();
    }
}
