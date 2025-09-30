using Xunit;
using MIXERX.Engine.Audio;

namespace MIXERX.Engine.Tests;

public class AudioBufferTests
{
    [Fact]
    public void AudioBuffer_Write_DoesNotBlock()
    {
        var buffer = new LockFreeAudioBuffer(1024);
        var data = new float[256];
        
        var start = DateTime.UtcNow;
        buffer.Write(data);
        var elapsed = DateTime.UtcNow - start;
        
        Assert.True(elapsed.TotalMilliseconds < 10);
    }

    [Fact]
    public void AudioBuffer_Read_ReturnsValidData()
    {
        var buffer = new LockFreeAudioBuffer(1024);
        var writeData = new float[] { 1.0f, 0.5f, -0.5f, -1.0f };
        var readData = new float[4];
        
        buffer.Write(writeData);
        var samplesRead = buffer.Read(readData);
        
        Assert.Equal(4, samplesRead);
        Assert.Equal(writeData, readData);
    }

    [Fact]
    public void AudioBuffer_Underrun_HandlesGracefully()
    {
        var buffer = new LockFreeAudioBuffer(1024);
        var readData = new float[256];
        
        var samplesRead = buffer.Read(readData);
        
        Assert.Equal(0, samplesRead);
        Assert.All(readData, sample => Assert.Equal(0.0f, sample));
    }
}
