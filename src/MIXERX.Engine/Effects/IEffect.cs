namespace MIXERX.Engine.Effects;

public interface IEffect
{
    void Process(Span<float> samples);
    void SetParameter(string name, float value);
    void Reset();
}
