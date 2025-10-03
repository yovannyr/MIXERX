namespace MIXERX.Core.Models;

public record MidiMessage(byte Status, byte Data1, byte Data2)
{
    public bool IsNoteOn(byte note) => (Status & 0xF0) == 0x90 && Data1 == note && Data2 > 0;
    public bool IsNoteOff(byte note) => (Status & 0xF0) == 0x80 && Data1 == note;
    public bool IsCC(byte controller) => (Status & 0xF0) == 0xB0 && Data1 == controller;
    public float Value => Data2 / 127.0f;
}