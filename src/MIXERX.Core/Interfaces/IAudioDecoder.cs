namespace MIXERX.Core.Interfaces;

public interface ICodecsAudioDecoder : IDisposable
{
    AudioData LoadFile(string path);
    int ReadSamples(float[] buffer, int offset, int count);
    int Read(float[] buffer, int offset, int count);
    bool Seek(TimeSpan position);
    
    int SampleRate { get; }
    int Channels { get; }
    TimeSpan Duration { get; }
}


public interface IAudioDecoder : IDisposable
{
    bool LoadFile(string filePath);
    int Read(float[] buffer, int offset, int count);
    void Seek(TimeSpan position);
    TimeSpan Duration { get; }
    int SampleRate { get; }
    int Channels { get; }
}
