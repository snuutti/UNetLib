using System.Numerics;
using System.Text;

namespace UNetLib.HLAPI;

public class NetworkWriter
{
    private readonly MemoryStream _stream;
    private readonly BinaryWriter _writer;

    private const int MaxStringLength = 1024 * 32;

    public NetworkWriter()
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

    public void Write(bool value)
    {
        _writer.Write(value);
    }

    public void Write(byte[] value)
    {
        _writer.Write(value);
    }

    public void Write(ushort value)
    {
        _writer.Write(value);
    }

    public void Write(float value)
    {
        _writer.Write(value);
    }

    public void Write(string? value)
    {
        if (value == null)
        {
            Write((ushort) 0);
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length >= MaxStringLength)
        {
            throw new IndexOutOfRangeException($"Write(string) too long: {bytes.Length}");
        }

        Write((ushort) bytes.Length);
        Write(bytes);
    }

    public void Write(Vector3 value)
    {
        Write(value.X);
        Write(value.Y);
        Write(value.Z);
    }

    public void Write(Quaternion value)
    {
        Write(value.X);
        Write(value.Y);
        Write(value.Z);
        Write(value.W);
    }

    public void WritePackedUInt32(uint value)
    {
        if (value <= 240)
        {
            Write((byte) value);
            return;
        }

        if (value <= 2287)
        {
            Write((byte) ((value - 240) / 256 + 241));
            Write((byte) ((value - 240) % 256));
            return;
        }

        if (value <= 67823)
        {
            Write(249);
            Write((byte) ((value - 2288) / 256));
            Write((byte) ((value - 2288) % 256));
            return;
        }

        if (value <= 16777215)
        {
            Write(250);
            Write((byte) (value & 0xFF));
            Write((byte) ((value >> 8) & 0xFF));
            Write((byte) ((value >> 16) & 0xFF));
            return;
        }

        // all other values of uint
        Write(251);
        Write((byte) (value & 0xFF));
        Write((byte) ((value >> 8) & 0xFF));
        Write((byte) ((value >> 16) & 0xFF));
        Write((byte) ((value >> 24) & 0xFF));
    }

    public void WriteBytesAndSize(byte[]? buffer, int count)
    {
        if (buffer == null || count == 0)
        {
            Write((ushort) 0);
            return;
        }

        Write((ushort) count);
        Write(buffer);
    }

    public void WriteBytesFull(byte[]? buffer)
    {
        WriteBytesAndSize(buffer, buffer?.Length ?? 0);
    }

    public void Write(NetworkInstanceId value)
    {
        WritePackedUInt32(value.Value);
    }

    public void Write(NetworkSceneId value)
    {
        WritePackedUInt32(value.Value);
    }

    public void Write(NetworkHash128 value)
    {
        Write(value.I0);
        Write(value.I1);
        Write(value.I2);
        Write(value.I3);
        Write(value.I4);
        Write(value.I5);
        Write(value.I6);
        Write(value.I7);
        Write(value.I8);
        Write(value.I9);
        Write(value.I10);
        Write(value.I11);
        Write(value.I12);
        Write(value.I13);
        Write(value.I14);
        Write(value.I15);
    }
}