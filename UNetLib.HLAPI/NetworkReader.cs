using System.Numerics;
using System.Text;

namespace UNetLib.HLAPI;

public class NetworkReader
{
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;

    public long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public long Length => _stream.Length;

    private const int MaxStringLength = 1024 * 32;

    public NetworkReader(byte[] buffer)
    {
        _stream = new MemoryStream(buffer);
        _reader = new BinaryReader(_stream);
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public bool ReadBoolean()
    {
        return _reader.ReadBoolean();
    }

    public byte[] ReadBytes(int count)
    {
        if (Length - Position < count)
        {
            throw new IndexOutOfRangeException($"ReadBytes() out of range: {count} bytes requested, {Length - Position} bytes available");
        }

        return _reader.ReadBytes(count);
    }

    public short ReadInt16()
    {
        return _reader.ReadInt16();
    }

    public ushort ReadUInt16()
    {
        return _reader.ReadUInt16();
    }

    public float ReadSingle()
    {
        return _reader.ReadSingle();
    }

    public uint ReadPackedUInt32()
    {
        var a0 = ReadByte();
        if (a0 < 241)
        {
            return a0;
        }

        var a1 = ReadByte();
        if (a0 <= 248)
        {
            return (uint) (240 + 256 * (a0 - 241) + a1);
        }

        var a2 = ReadByte();
        if (a0 == 249)
        {
            return (uint) (2288 + 256 * a1 + a2);
        }

        var a3 = ReadByte();
        if (a0 == 250)
        {
            return a1 + ((uint) a2 << 8) + ((uint) a3 << 16);
        }

        var a4 = ReadByte();
        return a1 + ((uint) a2 << 8) + ((uint) a3 << 16) + ((uint) a4 << 24);
    }

    public string ReadString()
    {
        var length = ReadUInt16();
        if (length >= MaxStringLength)
        {
            throw new IndexOutOfRangeException($"ReadString() too long: {length}");
        }

        var bytes = ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    public Vector3 ReadVector3()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        return new Vector3(x, y, z);
    }

    public Quaternion ReadQuaternion()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        var w = ReadSingle();
        return new Quaternion(x, y, z, w);
    }

    public byte[]? ReadBytesAndSize()
    {
        var length = ReadUInt16();
        if (length == 0)
        {
            return null;
        }

        return ReadBytes(length);
    }

    public NetworkInstanceId ReadNetworkId()
    {
        return new NetworkInstanceId(ReadPackedUInt32());
    }

    public NetworkSceneId ReadSceneId()
    {
        return new NetworkSceneId(ReadPackedUInt32());
    }

    public NetworkHash128 ReadNetworkHash128()
    {
        var hash = new NetworkHash128
        {
            I0 = ReadByte(),
            I1 = ReadByte(),
            I2 = ReadByte(),
            I3 = ReadByte(),
            I4 = ReadByte(),
            I5 = ReadByte(),
            I6 = ReadByte(),
            I7 = ReadByte(),
            I8 = ReadByte(),
            I9 = ReadByte(),
            I10 = ReadByte(),
            I11 = ReadByte(),
            I12 = ReadByte(),
            I13 = ReadByte(),
            I14 = ReadByte(),
            I15 = ReadByte()
        };

        return hash;
    }
}