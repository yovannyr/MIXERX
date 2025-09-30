#!/bin/bash
# MIXERX Iteration 3: WAV Decoder
# Zero-Break TDD Implementation

set -e

echo "🎯 Starting Iteration 3: WAV Decoder"

# 1. Create test WAV file
echo "📝 Creating test assets..."
mkdir -p tests/assets

# Create minimal WAV header + sine wave data
cat > tests/MIXERX.Engine.Tests/WavDecoderTests.cs << 'EOF'
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
EOF

# 2. Run tests (should fail)
echo "🔴 Running tests (should fail)..."
dotnet test tests/MIXERX.Engine.Tests/WavDecoderTests.cs || echo "✅ Tests failed as expected"

# 3. Green Phase: Minimal WAV decoder
echo "🟢 Implementing WAV decoder..."
mkdir -p src/MIXERX.Engine/Codecs

cat > src/MIXERX.Engine/Codecs/IAudioDecoder.cs << 'EOF'
namespace MIXERX.Engine.Codecs;

public interface IAudioDecoder
{
    AudioData LoadFile(string path);
    int ReadSamples(float[] buffer, int offset, int count);
}

public class AudioData
{
    public float[] Samples { get; set; } = Array.Empty<float>();
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public TimeSpan Duration { get; set; }
}
EOF

cat > src/MIXERX.Engine/Codecs/WavDecoder.cs << 'EOF'
using System.IO;

namespace MIXERX.Engine.Codecs;

public class WavDecoder : IAudioDecoder
{
    private AudioData? _currentAudio;
    private int _position;

    public AudioData LoadFile(string path)
    {
        if (!File.Exists(path))
            throw new InvalidDataException($"File not found: {path}");

        using var fs = new FileStream(path, FileMode.Open);
        using var reader = new BinaryReader(fs);

        // Read WAV header
        var riff = new string(reader.ReadChars(4));
        if (riff != "RIFF")
            throw new InvalidDataException("Invalid WAV file: Missing RIFF header");

        var fileSize = reader.ReadInt32();
        var wave = new string(reader.ReadChars(4));
        if (wave != "WAVE")
            throw new InvalidDataException("Invalid WAV file: Missing WAVE header");

        var fmt = new string(reader.ReadChars(4));
        if (fmt != "fmt ")
            throw new InvalidDataException("Invalid WAV file: Missing fmt chunk");

        var fmtSize = reader.ReadInt32();
        var audioFormat = reader.ReadInt16();
        var channels = reader.ReadInt16();
        var sampleRate = reader.ReadInt32();
        var byteRate = reader.ReadInt32();
        var blockAlign = reader.ReadInt16();
        var bitsPerSample = reader.ReadInt16();

        // Skip to data chunk
        var data = new string(reader.ReadChars(4));
        if (data != "data")
            throw new InvalidDataException("Invalid WAV file: Missing data chunk");

        var dataSize = reader.ReadInt32();
        var sampleCount = dataSize / (bitsPerSample / 8) / channels;

        // Read audio data
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            if (bitsPerSample == 16)
            {
                var sample = reader.ReadInt16();
                samples[i] = sample / 32768.0f;
            }
            else
            {
                throw new NotSupportedException($"Unsupported bit depth: {bitsPerSample}");
            }
        }

        _currentAudio = new AudioData
        {
            Samples = samples,
            SampleRate = sampleRate,
            Channels = channels,
            Duration = TimeSpan.FromSeconds((double)sampleCount / sampleRate)
        };

        _position = 0;
        return _currentAudio;
    }

    public int ReadSamples(float[] buffer, int offset, int count)
    {
        if (_currentAudio == null) return 0;

        var available = Math.Min(count, _currentAudio.Samples.Length - _position);
        Array.Copy(_currentAudio.Samples, _position, buffer, offset, available);
        _position += available;

        return available;
    }
}
EOF

# 4. Integrate with Deck
echo "🔧 Integrating decoder with Deck..."
cat >> src/MIXERX.Engine/Deck.cs << 'EOF'

    private IAudioDecoder? _decoder;
    private AudioData? _currentTrack;

    public bool LoadTrack(string filePath)
    {
        try
        {
            _decoder = new WavDecoder();
            _currentTrack = _decoder.LoadFile(filePath);
            _position = 0;
            return true;
        }
        catch
        {
            _decoder = null;
            _currentTrack = null;
            return false;
        }
    }

    public void GetAudioSamples(float[] buffer, int frames)
    {
        if (_decoder == null || _currentTrack == null || !IsPlaying)
        {
            Array.Fill(buffer, 0.0f);
            return;
        }

        var samplesRead = _decoder.ReadSamples(buffer, 0, frames);
        
        // Apply volume
        for (int i = 0; i < samplesRead; i++)
        {
            buffer[i] *= Volume;
        }

        // Fill remaining with silence
        for (int i = samplesRead; i < frames; i++)
        {
            buffer[i] = 0.0f;
        }
    }
EOF

# 5. Run tests (should pass)
echo "🟢 Running tests (should pass)..."
dotnet test tests/MIXERX.Engine.Tests/

# 6. Integration test
echo "🔧 Testing integration..."
dotnet build
dotnet test

echo "✅ Iteration 3 completed successfully!"
echo "📊 Status: WAV decoder implemented - first audio playback possible!"
echo "🎯 Next: Run ./scripts/iteration4.sh"
