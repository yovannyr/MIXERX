using System.Runtime.InteropServices;

namespace MIXERX.Infrastructure.Services.Audio;

public unsafe class LockFreeAudioBuffer : IDisposable
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

    public void Dispose()
    {
        if (_buffer != null)
            NativeMemory.Free(_buffer);
    }
}
