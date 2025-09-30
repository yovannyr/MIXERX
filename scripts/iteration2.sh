#!/bin/bash
# MIXERX Iteration 2: Audio Buffer System
# Zero-Break TDD Implementation

set -e

echo "🎯 Starting Iteration 2: Audio Buffer System"

# 1. Red Phase: Write failing tests
echo "📝 Creating buffer tests..."
cat > tests/MIXERX.Engine.Tests/AudioBufferTests.cs << 'EOF'
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
        
        Assert.True(elapsed.TotalMilliseconds < 1);
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
EOF

# 2. Run tests (should fail)
echo "🔴 Running tests (should fail)..."
dotnet test tests/MIXERX.Engine.Tests/AudioBufferTests.cs || echo "✅ Tests failed as expected"

# 3. Green Phase: Minimal implementation
echo "🟢 Implementing audio buffer..."
mkdir -p src/MIXERX.Engine/Audio

cat > src/MIXERX.Engine/Audio/LockFreeAudioBuffer.cs << 'EOF'
using System.Runtime.CompilerServices;

namespace MIXERX.Engine.Audio;

public unsafe class LockFreeAudioBuffer
{
    private readonly float* _buffer;
    private readonly int _size;
    private volatile int _writePos;
    private volatile int _readPos;

    public LockFreeAudioBuffer(int size)
    {
        _size = size;
        _buffer = (float*)NativeMemory.Alloc((nuint)(size * sizeof(float)));
        NativeMemory.Clear(_buffer, (nuint)(size * sizeof(float)));
    }

    public void Write(ReadOnlySpan<float> data)
    {
        var writePos = _writePos;
        var available = GetAvailableWrite();
        var toWrite = Math.Min(data.Length, available);

        for (int i = 0; i < toWrite; i++)
        {
            _buffer[(writePos + i) % _size] = data[i];
        }

        _writePos = (writePos + toWrite) % _size;
    }

    public int Read(Span<float> data)
    {
        var readPos = _readPos;
        var available = GetAvailableRead();
        var toRead = Math.Min(data.Length, available);

        for (int i = 0; i < toRead; i++)
        {
            data[i] = _buffer[(readPos + i) % _size];
        }

        // Fill remaining with silence
        for (int i = toRead; i < data.Length; i++)
        {
            data[i] = 0.0f;
        }

        _readPos = (readPos + toRead) % _size;
        return toRead;
    }

    private int GetAvailableWrite()
    {
        var writePos = _writePos;
        var readPos = _readPos;
        return writePos >= readPos ? _size - (writePos - readPos) - 1 : readPos - writePos - 1;
    }

    private int GetAvailableRead()
    {
        var writePos = _writePos;
        var readPos = _readPos;
        return writePos >= readPos ? writePos - readPos : _size - (readPos - writePos);
    }

    ~LockFreeAudioBuffer()
    {
        if (_buffer != null)
            NativeMemory.Free(_buffer);
    }
}
EOF

# 4. Update AudioEngine to use buffer
echo "🔧 Integrating buffer into AudioEngine..."
cat > src/MIXERX.Engine/AudioEngineBuffer.cs << 'EOF'
using MIXERX.Engine.Audio;

namespace MIXERX.Engine;

public partial class AudioEngine
{
    private LockFreeAudioBuffer? _masterBuffer;

    private void InitializeBuffers(AudioConfig config)
    {
        _masterBuffer = new LockFreeAudioBuffer(config.BufferSize * 4);
    }

    private void ProcessAudioBuffer(float[] outputBuffer, int frames)
    {
        if (_masterBuffer == null) return;

        // Mix all decks into master buffer
        var mixBuffer = new float[frames];
        foreach (var deck in _decks.Values)
        {
            if (deck.IsPlaying)
            {
                var deckBuffer = new float[frames];
                // TODO: Get audio from deck
                for (int i = 0; i < frames; i++)
                {
                    mixBuffer[i] += deckBuffer[i] * deck.Volume;
                }
            }
        }

        // Write to master buffer
        _masterBuffer.Write(mixBuffer);
        
        // Read from master buffer to output
        _masterBuffer.Read(outputBuffer);
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

echo "✅ Iteration 2 completed successfully!"
echo "📊 Status: Lock-free audio buffer system implemented"
echo "🎯 Next: Run ./scripts/iteration3.sh"
