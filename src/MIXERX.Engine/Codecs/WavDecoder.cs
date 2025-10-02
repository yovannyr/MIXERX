using System.IO;

namespace MIXERX.Engine.Codecs;

public class WavDecoder : IAudioDecoder
{
    private AudioData? _currentAudio;
    private int _position;

    public int SampleRate => _currentAudio?.SampleRate ?? 0;
    public int Channels => _currentAudio?.Channels ?? 0;
    public TimeSpan Duration => _currentAudio?.Duration ?? TimeSpan.Zero;

    public AudioData LoadFile(string path)
    {
        if (!File.Exists(path))
            throw new InvalidDataException($"File not found: {path}");

        using var fs = new FileStream(path, FileMode.Open);
        using var reader = new BinaryReader(fs);

        // Read WAV header
        var riff = new string(reader.ReadChars(4));
        if (riff != "RIFF")
            throw new InvalidDataException("Invalid WAV file: Missing RIFF header");

        var fileSize = reader.ReadInt32();
        var wave = new string(reader.ReadChars(4));
        if (wave != "WAVE")
            throw new InvalidDataException("Invalid WAV file: Missing WAVE header");

        var fmt = new string(reader.ReadChars(4));
        if (fmt != "fmt ")
            throw new InvalidDataException("Invalid WAV file: Missing fmt chunk");

        var fmtSize = reader.ReadInt32();
        var audioFormat = reader.ReadInt16();
        var channels = reader.ReadInt16();
        var sampleRate = reader.ReadInt32();
        var byteRate = reader.ReadInt32();
        var blockAlign = reader.ReadInt16();
        var bitsPerSample = reader.ReadInt16();

        // Skip to data chunk
        var data = new string(reader.ReadChars(4));
        if (data != "data")
            throw new InvalidDataException("Invalid WAV file: Missing data chunk");

        var dataSize = reader.ReadInt32();
        var sampleCount = dataSize / (bitsPerSample / 8) / channels;

        // Read audio data
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            if (bitsPerSample == 16)
            {
                var sample = reader.ReadInt16();
                samples[i] = sample / 32768.0f;
            }
            else
            {
                throw new NotSupportedException($"Unsupported bit depth: {bitsPerSample}");
            }
        }

        _currentAudio = new AudioData
        {
            Samples = samples,
            SampleRate = sampleRate,
            Channels = channels,
            Duration = TimeSpan.FromSeconds((double)sampleCount / sampleRate)
        };

        _position = 0;
        return _currentAudio;
    }

    public int ReadSamples(float[] buffer, int offset, int count)
    {
        if (_currentAudio == null) return 0;

        var available = Math.Min(count, _currentAudio.Samples.Length - _position);
        Array.Copy(_currentAudio.Samples, _position, buffer, offset, available);
        _position += available;

        return available;
    }

    public int Read(float[] buffer, int offset, int count) => ReadSamples(buffer, offset, count);

    public bool Seek(TimeSpan position)
    {
        if (_currentAudio == null) return false;
        
        var samplePosition = (int)(position.TotalSeconds * _currentAudio.SampleRate);
        _position = Math.Clamp(samplePosition, 0, _currentAudio.Samples.Length);
        return true;
    }

    public void Dispose()
    {
        _currentAudio = null;
    }
}
