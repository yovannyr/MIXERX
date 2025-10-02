using Xunit;
using MIXERX.Engine.Codecs;

namespace MIXERX.Engine.Tests;

public class AudioDecoderFactoryTests
{
    [Fact]
    public void Create_WavFile_ReturnsWavDecoder()
    {
        var decoder = AudioDecoderFactory.Create("test.wav");
        Assert.IsType<WavDecoder>(decoder);
    }

    [Theory]
    [InlineData("test.mp3")]
    [InlineData("test.flac")]
    [InlineData("test.aac")]
    [InlineData("test.ogg")]
    [InlineData("test.m4a")]
    public void Create_FFmpegSupportedFormats_ReturnsFFmpegDecoder(string filename)
    {
        var decoder = AudioDecoderFactory.Create(filename);
        Assert.IsType<FFmpegAudioDecoder>(decoder);
    }

    [Fact]
    public void Create_UnsupportedFormat_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => 
            AudioDecoderFactory.Create("test.xyz"));
    }

    [Theory]
    [InlineData("TEST.WAV")]
    [InlineData("test.MP3")]
    [InlineData("Test.FlAc")]
    public void Create_CaseInsensitive_ReturnsCorrectDecoder(string filename)
    {
        var decoder = AudioDecoderFactory.Create(filename);
        Assert.NotNull(decoder);
    }
}
