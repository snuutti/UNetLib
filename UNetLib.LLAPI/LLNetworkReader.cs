using System.Net;

namespace UNetLib.LLAPI;

public class LLNetworkReader
{
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;

    public long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public long Length => _stream.Length;

    public bool IsAtEnd => _stream.Position >= _stream.Length;

    public LLNetworkReader(byte[] buffer)
    {
        _stream = new MemoryStream(buffer);
        _reader = new BinaryReader(_stream);
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public byte[] ReadBytes(int count)
    {
        return _reader.ReadBytes(count);
    }

    public T ReadEnum<T>() where T : Enum
    {
        return (T) (object) ReadByte();
    }

    public ushort ReadUInt16()
    {
        return (ushort) IPAddress.NetworkToHostOrder(_reader.ReadInt16());
    }

    public uint ReadUInt32()
    {
        return (uint) IPAddress.NetworkToHostOrder(_reader.ReadInt32());
    }

    public ushort ReadMessageLength()
    {
        var firstByte = ReadByte();
        if ((firstByte & 0x80) == 0)
        {
            return firstByte;
        }

        var secondByte = ReadByte();
        return (ushort) (((secondByte & 0x7F) << 8) | firstByte);
    }
}