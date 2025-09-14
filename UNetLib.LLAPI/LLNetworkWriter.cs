using System.Net;

namespace UNetLib.LLAPI;

public class LLNetworkWriter
{
    private readonly MemoryStream _stream;
    private readonly BinaryWriter _writer;

    public LLNetworkWriter()
    {
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
    }

    public byte[] ToArray()
    {
        return _stream.ToArray();
    }

    public void Write(byte value)
    {
        _writer.Write(value);
    }

    public void Write(byte[] value)
    {
        _writer.Write(value);
    }

    public void Write<T>(T value) where T : Enum
    {
        _writer.Write(Convert.ToByte(value));
    }

    public void Write(ushort value)
    {
        _writer.Write((ushort) IPAddress.HostToNetworkOrder((short) value));
    }

    public void Write(uint value)
    {
        _writer.Write((uint) IPAddress.HostToNetworkOrder((int) value));
    }

    public void WriteMessageLength(ushort messageLength)
    {
        if (messageLength <= 0x7F)
        {
            Write((byte) messageLength);
            return;
        }

        var high = (byte) (((messageLength >> 8) & 0x7F) | 0x80);
        var low = (byte) (messageLength & 0xFF);
        Write(high);
        Write(low);
    }
}