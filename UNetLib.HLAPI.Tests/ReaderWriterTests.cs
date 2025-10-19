using System.Drawing;
using System.Numerics;

namespace UNetLib.HLAPI.Tests;

public class ReaderWriterTests
{
    [Test]
    public async Task ReadWrite_Char_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const char originalValue = 'A';

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadChar();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Byte_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const byte originalValue = 123;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadByte();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_SByte_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const sbyte originalValue = -100;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadSByte();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Boolean_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const bool originalValue = true;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadBoolean();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_ByteArray_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadBytes(originalValue.Length);

        // Assert
        await Assert.That(readValue).IsEquivalentTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Int16_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const short originalValue = 12345;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadInt16();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_UInt16_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const ushort originalValue = 12345;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadUInt16();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Int32_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const int originalValue = 123456789;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadInt32();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_UInt32_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const uint originalValue = 123456789;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadUInt32();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Int64_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const long originalValue = 1234567890123456789;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadInt64();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_UInt64_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const ulong originalValue = 12345678901234567890;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadUInt64();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Float_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const float originalValue = 123.456f;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadSingle();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Double_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const double originalValue = 123.456789;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadDouble();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_PackedUInt32_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const uint originalValue = 300;

        // Act
        writer.WritePackedUInt32(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadPackedUInt32();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_PackedUInt64_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const ulong originalValue = 70000;

        // Act
        writer.WritePackedUInt64(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadPackedUInt64();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_String_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        const string originalValue = "Hello, World!";

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadString();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Vector2_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new Vector2(1.0f, 2.0f);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadVector2();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Vector3_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new Vector3(1.0f, 2.0f, 3.0f);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadVector3();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Vector4_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadVector4();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Color_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = Color.FromArgb(255, 100, 150, 200);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadColor();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Color32_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = Color.FromArgb(255, 100, 150, 200);

        // Act
        writer.Write(originalValue, true);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadColor32();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Quaternion_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new Quaternion(1.0f, 2.0f, 3.0f, 4.0f);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadQuaternion();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Matrix4x4_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new Matrix4x4(
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 10.0f, 11.0f, 12.0f,
            13.0f, 14.0f, 15.0f, 16.0f);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadMatrix4x4();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_NetworkInstanceId_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new NetworkInstanceId(123456);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadNetworkId();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_NetworkSceneId_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var originalValue = new NetworkSceneId(654321);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadSceneId();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_NetworkHash128_IsSymmetrical()
    {
        // Arrange
        var writer = new NetworkWriter();
        var guid = Guid.NewGuid();
        var originalValue = NetworkHash128.Parse(guid);

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new NetworkReader(buffer);
        var readValue = reader.ReadNetworkHash128();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }
}