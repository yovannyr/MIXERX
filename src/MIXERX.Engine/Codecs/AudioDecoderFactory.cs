namespace MIXERX.Engine.Codecs;

public static class AudioDecoderFactory
{
    public static Codecs.IAudioDecoder Create(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        
        return extension switch
        {
            ".wav" => new WavDecoder(),
            ".mp3" or ".flac" or ".aac" or ".ogg" or ".m4a" => new FFmpegAudioDecoder(),
            _ => throw new NotSupportedException($"Unsupported audio format: {extension}")
        };
    }
}
