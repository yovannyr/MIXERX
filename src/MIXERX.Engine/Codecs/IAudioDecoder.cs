namespace MIXERX.Engine.Codecs;

public interface IAudioDecoder : IDisposable
{
    AudioData LoadFile(string path);
    int ReadSamples(float[] buffer, int offset, int count);
    int Read(float[] buffer, int offset, int count);
    bool Seek(TimeSpan position);
    
    int SampleRate { get; }
    int Channels { get; }
    TimeSpan Duration { get; }
}

public class AudioData
{
    public float[] Samples { get; set; } = Array.Empty<float>();
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public TimeSpan Duration { get; set; }
}
