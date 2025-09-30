using NUnit.Framework;
using MIXERX.Core;

namespace MIXERX.Core.Tests;

[TestFixture]
public class AudioEngineTests
{
    [Test]
    public void AudioConfig_DefaultValues_AreCorrect()
    {
        var config = new AudioConfig();
        
        Assert.That(config.SampleRate, Is.EqualTo(48000));
        Assert.That(config.BufferSize, Is.EqualTo(128));
        Assert.That(config.PreferredApi, Is.EqualTo(AudioApi.Default));
    }

    [Test]
    public void MidiMessage_IsNoteOn_DetectsCorrectly()
    {
        var noteOnMessage = new MidiMessage(0x90, 0x40, 127);
        var noteOffMessage = new MidiMessage(0x80, 0x40, 0);
        
        Assert.That(noteOnMessage.IsNoteOn(0x40), Is.True);
        Assert.That(noteOffMessage.IsNoteOn(0x40), Is.False);
    }

    [Test]
    public void MidiMessage_IsCC_DetectsCorrectly()
    {
        var ccMessage = new MidiMessage(0xB0, 0x07, 64);
        var noteMessage = new MidiMessage(0x90, 0x40, 127);
        
        Assert.That(ccMessage.IsCC(0x07), Is.True);
        Assert.That(noteMessage.IsCC(0x07), Is.False);
    }

    [Test]
    public void MidiMessage_Value_CalculatesCorrectly()
    {
        var message = new MidiMessage(0xB0, 0x07, 127);
        
        Assert.That(message.Value, Is.EqualTo(1.0f).Within(0.01f));
    }
}
