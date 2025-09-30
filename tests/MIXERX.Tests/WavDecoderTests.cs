using Xunit;
using MIXERX.Engine.Codecs;
using System.IO;

namespace MIXERX.Engine.Tests;

public class WavDecoderTests
{
    private readonly string _testWavPath;

    public WavDecoderTests()
    {
        _testWavPath = Path.Combine("tests", "assets", "test.wav");
        CreateTestWavFile();
    }

    [Fact]
    public void WavDecoder_LoadFile_ReturnsAudioData()
    {
        var decoder = new WavDecoder();
        var audioData = decoder.LoadFile(_testWavPath);
        
        Assert.NotNull(audioData);
        Assert.True(audioData.SampleRate > 0);
        Assert.True(audioData.Samples.Length > 0);
    }

    [Fact]
    public void WavDecoder_InvalidFile_ThrowsException()
    {
        var decoder = new WavDecoder();
        
        Assert.Throws<InvalidDataException>(() => 
            decoder.LoadFile("nonexistent.wav"));
    }

    [Fact]
    public void WavDecoder_ReadSamples_ReturnsCorrectCount()
    {
        var decoder = new WavDecoder();
        var audioData = decoder.LoadFile(_testWavPath);
        
        var samples = new float[100];
        var count = decoder.ReadSamples(samples, 0, 100);
        
        Assert.True(count > 0);
        Assert.True(count <= 100);
    }

    private void CreateTestWavFile()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_testWavPath)!);
        
        using var fs = new FileStream(_testWavPath, FileMode.Create);
        using var writer = new BinaryWriter(fs);
        
        // WAV Header (44 bytes)
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + 1000 * 2); // File size
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16); // PCM format size
        writer.Write((short)1); // PCM format
        writer.Write((short)1); // Mono
        writer.Write(44100); // Sample rate
        writer.Write(44100 * 2); // Byte rate
        writer.Write((short)2); // Block align
        writer.Write((short)16); // Bits per sample
        writer.Write("data".ToCharArray());
        writer.Write(1000 * 2); // Data size
        
        // Generate 1000 samples of sine wave
        for (int i = 0; i < 1000; i++)
        {
            var sample = (short)(Math.Sin(2 * Math.PI * 440 * i / 44100) * 16384);
            writer.Write(sample);
        }
    }
}
