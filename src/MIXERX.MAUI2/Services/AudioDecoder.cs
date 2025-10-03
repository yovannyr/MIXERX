//using NAudio.Wave;

namespace MIXERX.MAUI.Services;

public class AudioDecoder : IAudioDecoder
{
    private WaveStream? _waveStream;
    private ISampleProvider? _sampleProvider;

    public TimeSpan Duration => _waveStream?.TotalTime ?? TimeSpan.Zero;
    public int SampleRate => _waveStream?.WaveFormat.SampleRate ?? 44100;
    public int Channels => _waveStream?.WaveFormat.Channels ?? 2;

    public bool LoadFile(string filePath)
    {
        try
        {
            Dispose();

            var extension = Path.GetExtension(filePath).ToLower();
            
            _waveStream = extension switch
            {
                ".mp3" => new Mp3FileReader(filePath),
                ".wav" => new WaveFileReader(filePath),
                ".flac" => new FlacReader(filePath),
                _ => throw new NotSupportedException($"Unsupported audio format: {extension}")
            };

            // Convert to 32-bit float samples
            _sampleProvider = _waveStream.ToSampleProvider();
            
            // Note: Mono to stereo conversion would be handled here if needed

            return true;
        }
        catch
        {
            return false;
        }
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return _sampleProvider?.Read(buffer, offset, count) ?? 0;
    }

    public void Seek(TimeSpan position)
    {
        if (_waveStream != null)
        {
            _waveStream.CurrentTime = position;
        }
    }

    public void Dispose()
    {
        _waveStream?.Dispose();
        _waveStream = null;
        _sampleProvider = null;
    }
}

// Simple FLAC reader using NAudio
public class FlacReader : WaveStream
{
    private readonly WaveFileReader _reader;

    public FlacReader(string filePath)
    {
        // For now, assume FLAC files are converted to WAV
        // In production, use NAudio.Flac or similar
        _reader = new WaveFileReader(filePath);
    }

    public override WaveFormat WaveFormat => _reader.WaveFormat;
    public override long Length => _reader.Length;
    public override long Position 
    { 
        get => _reader.Position; 
        set => _reader.Position = value; 
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _reader.Read(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _reader?.Dispose();
        }
        base.Dispose(disposing);
    }
}
