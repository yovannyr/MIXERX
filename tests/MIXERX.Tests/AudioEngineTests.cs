using Xunit;
using MIXERX.Core;
using MIXERX.Core.Interfaces;

namespace MIXERX.Tests;

public class AudioEngineTests
{
    [Fact]
    public void AudioConfig_DefaultValues_AreCorrect()
    {
        var config = new AudioConfig();

        Assert.Equal(48000, config.SampleRate);
        Assert.Equal(128, config.BufferSize);
        Assert.Equal(AudioApi.Default, config.PreferredApi);
    }

    [Fact]
    public void MidiMessage_IsNoteOn_DetectsCorrectly()
    {
        var noteOnMessage = new MidiMessage(0x90, 0x40, 127);
        var noteOffMessage = new MidiMessage(0x80, 0x40, 0);

        Assert.True(noteOnMessage.IsNoteOn(0x40));
        Assert.False(noteOffMessage.IsNoteOn(0x40));
    }

    [Fact]
    public void MidiMessage_IsCC_DetectsCorrectly()
    {
        var ccMessage = new MidiMessage(0xB0, 0x07, 64);
        var noteMessage = new MidiMessage(0x90, 0x40, 127);

        Assert.True(ccMessage.IsCC(0x07));
        Assert.False(noteMessage.IsCC(0x07));
    }

    [Fact]
    public void MidiMessage_Value_CalculatesCorrectly()
    {
        var message = new MidiMessage(0xB0, 0x07, 127);

        Assert.Equal(1.0f, message.Value, 2);
    }
}
