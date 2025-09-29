using System.Net;
using UNetLib.LLAPI;
using UNetLib.LLAPI.Packet;
using UNetLib.LLAPI.Utils;

namespace UNetLib;

public class UNetClient
{
    private readonly UNetServer _server;

    private readonly IPEndPoint _remoteEndPoint;

    private readonly ushort _connectionId;

    private readonly ushort _remoteConnectionId;

    private readonly ushort _sessionId;

    private readonly ushort _pingSessionId;

    private ushort _nextPacketId;

    private ushort _nextMessageId;

    public ConnectionState State { get; set; }

    public IPEndPoint RemoteEndPoint => _remoteEndPoint;

    public ushort ConnectionId => _connectionId;

    public UNetClient(UNetServer server, IPEndPoint remoteEndPoint, ushort connectionId, ushort remoteConnectionId,
        ushort sessionId, ushort remoteSessionId)
    {
        _server = server;
        _remoteEndPoint = remoteEndPoint;
        _connectionId = connectionId;
        _remoteConnectionId = remoteConnectionId;
        _sessionId = sessionId;
        _pingSessionId = ConnectionUtils.CalculateRemoteSessionId(remoteSessionId);
    }

    internal void ProcessPing(PingPacket incomingPing)
    {
        SendPing(incomingPing);
    }

    public void Send(byte[] data)
    {
        _server.Send(_remoteEndPoint, data);
    }

    public void Send(LLNetworkWriter writer)
    {
        Send(writer.ToArray());
    }

    public void SendByChannel(byte[] data, byte channelId)
    {
        var writer = new LLNetworkWriter();
        BuildPacket(writer, channelId, data);
        Send(writer);
    }

    private void BuildPacket(LLNetworkWriter writer, byte channelId, byte[]? data)
    {
        writer.Write(_connectionId);
        writer.Write(_nextPacketId);
        writer.Write(_sessionId);
        writer.Write((ushort) 0); // TODO: AckMessageId
        writer.Write([0xFF, 0xFF, 0xFF, 0xFF]); // TODO: Acks

        if (_server.Config.IsAcksLong)
        {
            writer.Write([0xFF, 0xFF, 0xFF, 0xFF]);
        }

        if (data == null || data.Length == 0)
        {
            return;
        }

        writer.Write(channelId);

        var messageLength = (ushort) data.Length;
        var qosType = _server.Config.GetChannelType(channelId);
        var isReliable = ChannelUtils.IsChannelReliable(qosType);
        var isSequenced = ChannelUtils.IsChannelSequenced(qosType);

        if (isReliable)
        {
            messageLength += 2;
        }

        if (isSequenced)
        {
            messageLength += 1;
        }

        writer.Write(messageLength);

        if (isReliable)
        {
            writer.Write(_nextMessageId);
        }

        if (isSequenced)
        {
            writer.Write(0); // TODO: OrderedMessageId
        }

        writer.Write(data);
        _nextPacketId++;
        _nextMessageId++;
    }

    internal void SendPing(PingPacket? incomingPing = null)
    {
        var packet = new PingPacket
        {
            ConnectionId = 0,
            RequestType = SystemPacket.SystemRequestType.Ping,
            PacketId = _nextPacketId,
            SessionId = _sessionId,
            LocalConnectionId = _connectionId,
            RemoteConnectionId = _remoteConnectionId,
            SentPingTime = (uint) Environment.TickCount,
            AckPingTime = incomingPing?.SentPingTime ?? 0,
            LocalTimeCorrection = 0,// TODO
            IntDropRate = 0,// TODO
            ExtDropRate = 0,// TODO
            RemoteSessionId = _pingSessionId
        };

        _nextPacketId++;

        var writer = new LLNetworkWriter();
        packet.Serialize(writer);
        Send(writer);
    }

    internal void SendDisconnect(DisconnectPacket.DisconnectReason reason = DisconnectPacket.DisconnectReason.Ok)
    {
        var packet = new DisconnectPacket
        {
            ConnectionId = 0,
            RequestType = SystemPacket.SystemRequestType.Disconnect,
            PacketId = 0,
            SessionId = _sessionId,
            LocalConnectionId = _connectionId,
            RemoteConnectionId = _remoteConnectionId,
            LibVersion = 16777472,
            Reason = reason
        };

        var writer = new LLNetworkWriter();
        packet.Serialize(writer);
        Send(writer);
    }

    public enum ConnectionState
    {
        Connected,
        Disconnected
    }
}