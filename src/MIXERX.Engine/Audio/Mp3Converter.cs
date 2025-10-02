using FFMpegCore;

namespace MIXERX.Engine.Audio;

public static class Mp3Converter
{
    public static async Task<bool> ConvertWavToMp3Async(string wavPath, string mp3Path, int bitrate = 192)
    {
        try
        {
            await FFMpegArguments
                .FromFileInput(wavPath)
                .OutputToFile(mp3Path, overwrite: true, options => options
                    .WithAudioCodec("libmp3lame")
                    .WithAudioBitrate(bitrate))
                .ProcessAsynchronously();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
