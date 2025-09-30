namespace MIXERX.Engine.Codecs;

public interface IAudioDecoder
{
    AudioData LoadFile(string path);
    int ReadSamples(float[] buffer, int offset, int count);
}

public class AudioData
{
    public float[] Samples { get; set; } = Array.Empty<float>();
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public TimeSpan Duration { get; set; }
}
