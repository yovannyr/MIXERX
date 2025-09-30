using System.IO.MemoryMappedFiles;

namespace MIXERX.Core.IPC;

public class SharedMemoryBuffer : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly MemoryMappedViewAccessor _accessor;
    private readonly string _name;
    private readonly int _size;

    public SharedMemoryBuffer(string name, int size)
    {
        _name = name;
        _size = size;
        
        try
        {
            _mmf = MemoryMappedFile.CreateOrOpen(name, size);
            _accessor = _mmf.CreateViewAccessor(0, size);
        }
        catch
        {
            _mmf = MemoryMappedFile.CreateNew(name, size);
            _accessor = _mmf.CreateViewAccessor(0, size);
        }
    }

    public void WriteFloat(int offset, float value)
    {
        _accessor.Write(offset * 4, value);
    }

    public float ReadFloat(int offset)
    {
        return _accessor.ReadSingle(offset * 4);
    }

    public void WriteFloatArray(int offset, float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            WriteFloat(offset + i, data[i]);
        }
    }

    public void ReadFloatArray(int offset, float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = ReadFloat(offset + i);
        }
    }

    public void WriteInt(int offset, int value)
    {
        _accessor.Write(offset, value);
    }

    public int ReadInt(int offset)
    {
        return _accessor.ReadInt32(offset);
    }

    public void Dispose()
    {
        _accessor?.Dispose();
        _mmf?.Dispose();
    }
}

public static class SharedMemoryLayout
{
    public const int HEADER_SIZE = 64;
    public const int BUFFER_SIZE_OFFSET = 0;
    public const int SAMPLE_RATE_OFFSET = 4;
    public const int CHANNELS_OFFSET = 8;
    public const int WRITE_POSITION_OFFSET = 12;
    public const int READ_POSITION_OFFSET = 16;
    public const int AUDIO_DATA_OFFSET = HEADER_SIZE;
}
