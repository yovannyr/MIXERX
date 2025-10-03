//using FFMpegCore;
//using FFMpegCore.Pipes;
//using MIXERX.Core;
//using System.IO;
//using MIXERX.Core.Interfaces;
//using MIXERX.Core.Models;

namespace MIXERX.Infrastructure.Services.Codecs;

public class FFmpegAudioDecoder : ICodecsAudioDecoder
{
    private float[]? _samples;
    private int _sampleRate = 48000;
    private int _channels = 2;
    private TimeSpan _duration;
    private int _position;

    public int SampleRate => _sampleRate;
    public int Channels => _channels;
    public TimeSpan Duration => _duration;

    public AudioData LoadFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        
        // Use FFmpeg for MP3, FLAC, AAC, OGG
        if (extension is ".mp3" or ".flac" or ".aac" or ".ogg" or ".m4a")
        {
            if (LoadWithFFmpeg(filePath))
            {
                return new AudioData
                {
                    Samples = _samples!,
                    SampleRate = _sampleRate,
                    Channels = _channels,
                    Duration = _duration
                };
            }
            throw new InvalidDataException($"Failed to decode audio file: {filePath}");
        }
        
        // Fallback to WAV decoder for WAV files
        if (extension == ".wav")
        {
            var wavDecoder = new WavDecoder();
            return wavDecoder.LoadFile(filePath);
        }
        
        throw new NotSupportedException($"Unsupported audio format: {extension}");
    }

    private bool LoadWithFFmpeg(string filePath)
    {
        try
        {
            var outputStream = new MemoryStream();
            
            // Convert to PCM using FFmpeg
            //FFMpegArguments
            //    .FromFileInput(filePath)
            //    .OutputToPipe(new StreamPipeSink(outputStream), options => options
            //        .WithAudioCodec("pcm_f32le")
            //        .WithAudioSamplingRate(_sampleRate)
            //        .WithCustomArgument("-ac 2") // Force stereo
            //        .ForceFormat("f32le"))
            //    .ProcessSynchronously();

            // Convert byte array to float array
            var bytes = outputStream.ToArray();
            _samples = new float[bytes.Length / 4]; // 4 bytes per float
            
            Buffer.BlockCopy(bytes, 0, _samples, 0, bytes.Length);
            
            _channels = 2; // Forced stereo
            _duration = TimeSpan.FromSeconds(_samples.Length / (double)(_sampleRate * _channels));
            _position = 0;
            
            return _samples.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    public int ReadSamples(float[] buffer, int offset, int count)
    {
        if (_samples == null) return 0;
        
        var available = Math.Min(count, _samples.Length - _position);
        if (available <= 0) return 0;
        
        Array.Copy(_samples, _position, buffer, offset, available);
        _position += available;
        
        return available;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return ReadSamples(buffer, offset, count);
    }

    public bool Seek(TimeSpan position)
    {
        if (_samples == null) return false;
        
        var samplePosition = (int)(position.TotalSeconds * _sampleRate * _channels);
        _position = Math.Clamp(samplePosition, 0, _samples.Length);
        
        return true;
    }

    public void Dispose()
    {
        _samples = null;
    }
}
