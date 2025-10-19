using System.Drawing;
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

    public char ReadChar()
    {
        return _reader.ReadChar();
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public sbyte ReadSByte()
    {
        return _reader.ReadSByte();
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

    public int ReadInt32()
    {
        return _reader.ReadInt32();
    }

    public uint ReadUInt32()
    {
        return _reader.ReadUInt32();
    }

    public long ReadInt64()
    {
        return _reader.ReadInt64();
    }

    public ulong ReadUInt64()
    {
        return _reader.ReadUInt64();
    }

    public float ReadSingle()
    {
        return _reader.ReadSingle();
    }

    public double ReadDouble()
    {
        return _reader.ReadDouble();
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

    public ulong ReadPackedUInt64()
    {
        var a0 = ReadByte();
        if (a0 < 241)
        {
            return a0;
        }

        var a1 = ReadByte();
        if (a0 <= 248)
        {
            return 240 + 256 * (a0 - (ulong) 241) + a1;
        }

        var a2 = ReadByte();
        if (a0 == 249)
        {
            return 2288 + (ulong) 256 * a1 + a2;
        }

        var a3 = ReadByte();
        if (a0 == 250)
        {
            return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16);
        }

        var a4 = ReadByte();
        if (a0 == 251)
        {
            return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16) + ((ulong) a4 << 24);
        }

        var a5 = ReadByte();
        if (a0 == 252)
        {
            return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16) + ((ulong) a4 << 24) + ((ulong) a5 << 32);
        }

        var a6 = ReadByte();
        if (a0 == 253)
        {
            return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16) + ((ulong) a4 << 24) + ((ulong) a5 << 32) + ((ulong) a6 << 40);
        }

        var a7 = ReadByte();
        if (a0 == 254)
        {
            return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16) + ((ulong) a4 << 24) + ((ulong) a5 << 32) + ((ulong) a6 << 40) + ((ulong) a7 << 48);
        }

        var a8 = ReadByte();
        return a1 + ((ulong) a2 << 8) + ((ulong) a3 << 16) + ((ulong) a4 << 24) + ((ulong) a5 << 32) + ((ulong) a6 << 40) + ((ulong) a7 << 48) + ((ulong) a8 << 56);
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

    public Vector2 ReadVector2()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        return new Vector2(x, y);
    }

    public Vector3 ReadVector3()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        return new Vector3(x, y, z);
    }

    public Vector4 ReadVector4()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        var w = ReadSingle();
        return new Vector4(x, y, z, w);
    }

    public Color ReadColor()
    {
        var r = (int) ReadSingle();
        var g = (int) ReadSingle();
        var b = (int) ReadSingle();
        var a = (int) ReadSingle();
        return Color.FromArgb(a, r, g, b);
    }

    public Color ReadColor32()
    {
        var r = ReadByte();
        var g = ReadByte();
        var b = ReadByte();
        var a = ReadByte();
        return Color.FromArgb(a, r, g, b);
    }

    public Quaternion ReadQuaternion()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        var w = ReadSingle();
        return new Quaternion(x, y, z, w);
    }

    public Matrix4x4 ReadMatrix4x4()
    {
        var m11 = ReadSingle();
        var m12 = ReadSingle();
        var m13 = ReadSingle();
        var m14 = ReadSingle();
        var m21 = ReadSingle();
        var m22 = ReadSingle();
        var m23 = ReadSingle();
        var m24 = ReadSingle();
        var m31 = ReadSingle();
        var m32 = ReadSingle();
        var m33 = ReadSingle();
        var m34 = ReadSingle();
        var m41 = ReadSingle();
        var m42 = ReadSingle();
        var m43 = ReadSingle();
        var m44 = ReadSingle();

        return new Matrix4x4(
            m11, m12, m13, m14,
            m21, m22, m23, m24,
            m31, m32, m33, m34,
            m41, m42, m43, m44);
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