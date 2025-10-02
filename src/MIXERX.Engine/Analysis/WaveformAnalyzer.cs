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
}
