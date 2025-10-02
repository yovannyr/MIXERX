namespace MIXERX.Engine.Analysis;

public static class WaveformAnalyzer
{
    public static float[] AnalyzeWaveform(float[] audioData, int targetSamples = 1000)
    {
        if (audioData == null || audioData.Length == 0)
            return Array.Empty<float>();

        if (audioData.Length <= targetSamples)
            return audioData;

        var waveform = new float[targetSamples];
        var samplesPerBucket = audioData.Length / targetSamples;

        for (int i = 0; i < targetSamples; i++)
        {
            var start = i * samplesPerBucket;
            var end = Math.Min(start + samplesPerBucket, audioData.Length);
            
            // Peak detection: Find max absolute amplitude in bucket
            float maxAmplitude = 0;
            for (int j = start; j < end; j++)
            {
                var amplitude = Math.Abs(audioData[j]);
                if (amplitude > maxAmplitude)
                    maxAmplitude = amplitude;
            }
            
            waveform[i] = maxAmplitude * (audioData[start] >= 0 ? 1 : -1);
        }

        return waveform;
    }

    public static float[] CalculateEnergyLevels(float[] audioData, int segments = 100)
    {
        if (audioData == null || audioData.Length == 0)
            return Array.Empty<float>();

        var energyLevels = new float[segments];
        var samplesPerSegment = audioData.Length / segments;

        for (int i = 0; i < segments; i++)
        {
            var start = i * samplesPerSegment;
            var end = Math.Min(start + samplesPerSegment, audioData.Length);
            
            // Calculate RMS (Root Mean Square) energy
            float sumSquares = 0;
            for (int j = start; j < end; j++)
            {
                sumSquares += audioData[j] * audioData[j];
            }
            
            energyLevels[i] = MathF.Sqrt(sumSquares / (end - start));
        }

        // Normalize to 0.0 - 1.0 range
        var maxEnergy = energyLevels.Max();
        if (maxEnergy > 0)
        {
            for (int i = 0; i < segments; i++)
            {
                energyLevels[i] /= maxEnergy;
            }
        }

        return energyLevels;
    }
}
