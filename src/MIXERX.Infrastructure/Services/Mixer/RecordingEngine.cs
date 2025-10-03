namespace MIXERX.Infrastructure.Services.Mixer;

public class RecordingEngine : IDisposable
{
    private FileStream? _fileStream;
    private BinaryWriter? _writer;
    private bool _isRecording;
    private int _sampleRate;
    private int _channels;
    private long _samplesWritten;

    public bool IsRecording => _isRecording;
    public TimeSpan RecordingDuration => TimeSpan.FromSeconds((double)_samplesWritten / _sampleRate / _channels);

    public bool StartRecording(string filePath, int sampleRate = 48000, int channels = 2)
    {
        if (_isRecording) return false;

        try
        {
            _sampleRate = sampleRate;
            _channels = channels;
            _samplesWritten = 0;

            _fileStream = new FileStream(filePath, FileMode.Create);
            _writer = new BinaryWriter(_fileStream);

            // Write WAV header (will update later with final size)
            WriteWavHeader(_writer, 0);

            _isRecording = true;
            return true;
        }
        catch
        {
            Dispose();
            return false;
        }
    }

    public void ProcessAudio(Span<float> samples)
    {
        if (!_isRecording || _writer == null) return;

        // Convert float samples to 16-bit PCM
        foreach (var sample in samples)
        {
            var pcmSample = (short)(Math.Clamp(sample, -1.0f, 1.0f) * 32767);
            _writer.Write(pcmSample);
            _samplesWritten++;
        }
    }

    public void StopRecording()
    {
        if (!_isRecording) return;

        _isRecording = false;

        // Update WAV header with final size
        if (_writer != null && _fileStream != null)
        {
            var dataSize = _samplesWritten * 2; // 16-bit = 2 bytes per sample
            _fileStream.Seek(0, SeekOrigin.Begin);
            WriteWavHeader(_writer, dataSize);
        }

        Dispose();
    }

    private void WriteWavHeader(BinaryWriter writer, long dataSize)
    {
        var byteRate = _sampleRate * _channels * 2; // 16-bit
        var blockAlign = (short)(_channels * 2);

        writer.Write("RIFF".ToCharArray());
        writer.Write((int)(36 + dataSize));
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16); // PCM format size
        writer.Write((short)1); // PCM format
        writer.Write((short)_channels);
        writer.Write(_sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write((short)16); // Bits per sample
        writer.Write("data".ToCharArray());
        writer.Write((int)dataSize);
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _fileStream?.Dispose();
        _writer = null;
        _fileStream = null;
    }
}
