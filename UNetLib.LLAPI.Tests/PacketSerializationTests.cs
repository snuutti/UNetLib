using UNetLib.LLAPI.Packet;

namespace UNetLib.LLAPI.Tests;

public class PacketSerializationTests
{
    [Test]
    public async Task ConnectPacket_Serialization_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        var originalPacket = new ConnectPacket
        {
            ConnectionId = 42,
            RequestType = SystemPacket.SystemRequestType.ConnectRequest,
            PacketId = 1,
            SessionId = 100,
            LocalConnectionId = 200,
            RemoteConnectionId = 300,
            LibVersion = 123456,
            Crc = 654321
        };

        // Act
        originalPacket.Serialize(writer);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var deserializedPacket = new ConnectPacket();
        deserializedPacket.Deserialize(reader);

        // Assert
        using var _ = Assert.Multiple();
        await Assert.That(deserializedPacket.ConnectionId).IsEqualTo(originalPacket.ConnectionId);
        await Assert.That(deserializedPacket.RequestType).IsEqualTo(originalPacket.RequestType);
        await Assert.That(deserializedPacket.PacketId).IsEqualTo(originalPacket.PacketId);
        await Assert.That(deserializedPacket.SessionId).IsEqualTo(originalPacket.SessionId);
        await Assert.That(deserializedPacket.LocalConnectionId).IsEqualTo(originalPacket.LocalConnectionId);
        await Assert.That(deserializedPacket.RemoteConnectionId).IsEqualTo(originalPacket.RemoteConnectionId);
        await Assert.That(deserializedPacket.LibVersion).IsEqualTo(originalPacket.LibVersion);
        await Assert.That(deserializedPacket.Crc).IsEqualTo(originalPacket.Crc);
    }

    [Test]
    public async Task DisconnectPacket_Serialization_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        var originalPacket = new DisconnectPacket
        {
            ConnectionId = 42,
            RequestType = SystemPacket.SystemRequestType.Disconnect,
            PacketId = 1,
            SessionId = 100,
            LibVersion = 123456,
            Reason = DisconnectPacket.DisconnectReason.Timeout
        };

        // Act
        originalPacket.Serialize(writer);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var deserializedPacket = new DisconnectPacket();
        deserializedPacket.Deserialize(reader);

        // Assert
        using var _ = Assert.Multiple();
        await Assert.That(deserializedPacket.ConnectionId).IsEqualTo(originalPacket.ConnectionId);
        await Assert.That(deserializedPacket.RequestType).IsEqualTo(originalPacket.RequestType);
        await Assert.That(deserializedPacket.PacketId).IsEqualTo(originalPacket.PacketId);
        await Assert.That(deserializedPacket.SessionId).IsEqualTo(originalPacket.SessionId);
        await Assert.That(deserializedPacket.LibVersion).IsEqualTo(originalPacket.LibVersion);
        await Assert.That(deserializedPacket.Reason).IsEqualTo(originalPacket.Reason);
    }

    [Test]
    public async Task PingPacket_Serialization_IsSymmetrical()
    {
        // Arrange
        var writer = new LLNetworkWriter();
        var originalPacket = new PingPacket
        {
            ConnectionId = 42,
            RequestType = SystemPacket.SystemRequestType.Ping,
            PacketId = 1,
            SessionId = 100,
            SentPingTime = 1111,
            AckPingTime = 2222,
            LocalTimeCorrection = 3333,
            IntDropRate = 10,
            ExtDropRate = 20,
            RemoteSessionId = 300
        };

        // Act
        originalPacket.Serialize(writer);
        var buffer = writer.ToArray();
        var reader = new LLNetworkReader(buffer);
        var deserializedPacket = new PingPacket();
        deserializedPacket.Deserialize(reader);

        // Assert
        using var _ = Assert.Multiple();
        await Assert.That(deserializedPacket.ConnectionId).IsEqualTo(originalPacket.ConnectionId);
        await Assert.That(deserializedPacket.RequestType).IsEqualTo(originalPacket.RequestType);
        await Assert.That(deserializedPacket.PacketId).IsEqualTo(originalPacket.PacketId);
        await Assert.That(deserializedPacket.SessionId).IsEqualTo(originalPacket.SessionId);
        await Assert.That(deserializedPacket.SentPingTime).IsEqualTo(originalPacket.SentPingTime);
        await Assert.That(deserializedPacket.AckPingTime).IsEqualTo(originalPacket.AckPingTime);
        await Assert.That(deserializedPacket.LocalTimeCorrection).IsEqualTo(originalPacket.LocalTimeCorrection);
        await Assert.That(deserializedPacket.IntDropRate).IsEqualTo(originalPacket.IntDropRate);
        await Assert.That(deserializedPacket.ExtDropRate).IsEqualTo(originalPacket.ExtDropRate);
        await Assert.That(deserializedPacket.RemoteSessionId).IsEqualTo(originalPacket.RemoteSessionId);
    }
}