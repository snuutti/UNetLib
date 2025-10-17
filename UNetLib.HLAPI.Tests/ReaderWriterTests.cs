using System.Numerics;

namespace UNetLib.HLAPI.Tests;

public class ReaderWriterTests
{
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