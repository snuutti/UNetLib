using UNetLib.LLAPI.Packet;

namespace UNetLib.LLAPI.Tests;

public class ReaderWriterTests
{
    [Test]
    public async Task ReadWrite_Byte_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        const byte originalValue = 123;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var readValue = reader.ReadByte();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_ByteArray_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        var originalValue = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var readValue = reader.ReadBytes(originalValue.Length);

        // Assert
        await Assert.That(readValue).IsEquivalentTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_Enum_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        const SystemPacket.SystemRequestType originalValue = SystemPacket.SystemRequestType.Ping;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var readValue = reader.ReadEnum<SystemPacket.SystemRequestType>();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_UInt16_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        const ushort originalValue = 54321;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var readValue = reader.ReadUInt16();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }

    [Test]
    public async Task ReadWrite_UInt32_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        const uint originalValue = 1234567890;

        // Act
        writer.Write(originalValue);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var readValue = reader.ReadUInt32();

        // Assert
        await Assert.That(readValue).IsEqualTo(originalValue);
    }
}