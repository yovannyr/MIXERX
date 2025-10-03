namespace MIXERX.Infrastructure.Services.Mixer;

public class AudioMeter
{
    private float _peakLeft;
    private float _peakRight;
    private float _rmsLeft;
    private float _rmsRight;
    private const float DecayRate = 0.95f;

    public void Process(Span<float> samples)
    {
        if (samples.Length == 0) return;

        float sumSquaredLeft = 0;
        float sumSquaredRight = 0;
        float maxLeft = 0;
        float maxRight = 0;
        int sampleCount = samples.Length / 2;

        for (int i = 0; i < samples.Length; i += 2)
        {
            var left = Math.Abs(samples[i]);
            var right = i + 1 < samples.Length ? Math.Abs(samples[i + 1]) : left;

            maxLeft = Math.Max(maxLeft, left);
            maxRight = Math.Max(maxRight, right);
            
            sumSquaredLeft += left * left;
            sumSquaredRight += right * right;
        }

        // Update peak with decay
        _peakLeft = Math.Max(maxLeft, _peakLeft * DecayRate);
        _peakRight = Math.Max(maxRight, _peakRight * DecayRate);

        // Calculate RMS
        _rmsLeft = MathF.Sqrt(sumSquaredLeft / sampleCount);
        _rmsRight = MathF.Sqrt(sumSquaredRight / sampleCount);
    }

    public MeterLevels GetLevels()
    {
        return new MeterLevels
        {
            PeakLeft = AmplitudeToDb(_peakLeft),
            PeakRight = AmplitudeToDb(_peakRight),
            RmsLeft = AmplitudeToDb(_rmsLeft),
            RmsRight = AmplitudeToDb(_rmsRight)
        };
    }

    public void Reset()
    {
        _peakLeft = _peakRight = 0;
        _rmsLeft = _rmsRight = 0;
    }

    private static float AmplitudeToDb(float amplitude)
    {
        if (amplitude <= 0.0001f) return -60f; // Silence threshold
        return 20 * MathF.Log10(amplitude);
    }
}
