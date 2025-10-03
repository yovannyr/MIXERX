namespace MIXERX.Core.Interfaces;

public interface IAudioNode
{
    void Process(Span<float> input, Span<float> output, int sampleCount);
    void SetParameter(string name, float value);
    void Reset();
}