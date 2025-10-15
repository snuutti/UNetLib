using System.Net;
using UNetLib.Channel;
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

    private readonly BaseChannel[] _channels;

    private readonly PacketAcks _packetAcks;

    private readonly Dictionary<ushort, (byte channelId, byte[] data)> _pendingReliableMessages = new();

    private ushort _nextPacketId;

    private ushort _nextMessageId = 1;

    public ConnectionState State { get; set; }

    public IPEndPoint RemoteEndPoint => _remoteEndPoint;

    public ushort ConnectionId => _connectionId;

    internal PacketAcks PacketAcks => _packetAcks;

    internal IUNetEventListener EventListener => _server.EventListener;

    public UNetClient(UNetServer server, IPEndPoint remoteEndPoint, ushort connectionId, ushort remoteConnectionId,
        ushort sessionId, ushort remoteSessionId)
    {
        _server = server;
        _remoteEndPoint = remoteEndPoint;
        _connectionId = connectionId;
        _remoteConnectionId = remoteConnectionId;
        _sessionId = sessionId;
        _pingSessionId = ConnectionUtils.CalculateRemoteSessionId(remoteSessionId);
        _channels = new BaseChannel[server.Config.Channels.Count];

        for (byte i = 0; i < _channels.Length; i++)
        {
            var qosType = server.Config.GetChannelType(i);
            _channels[i] = qosType switch
            {
                QosType.Unreliable => new UnreliableChannel(this, i),
                QosType.UnreliableSequenced => new UnreliableSequencedChannel(this, i),
                QosType.Reliable => new ReliableChannel(this, i),
                QosType.ReliableSequenced => new ReliableSequencedChannel(this, i),
                _ => throw new NotSupportedException($"QosType {qosType} is not supported!")
            };
        }

        _packetAcks = new PacketAcks(server.Config.IsAcksLong);
    }

    internal void ProcessPing(PingPacket incomingPing)
    {
        if (State == ConnectionState.Handshake)
        {
            State = ConnectionState.Connected;
            EventListener.OnClientConnected(this);
        }

        SendPing(incomingPing);
    }

    internal void ProcessDataPacket(LLNetworkReader reader)
    {
        var channelId = reader.ReadByte();
        if (channelId >= _channels.Length)
        {
            Console.WriteLine($"Invalid channel ID {channelId} from {_remoteEndPoint}!");
            return;
        }

        _channels[channelId].Process(reader);
    }

    internal void StoreReliableMessage(ushort messageId, byte channelId, byte[] data)
    {
        _pendingReliableMessages[messageId] = (channelId, data);
    }

    internal void ProcessAcks(ushort ackMessageId, uint[] acks)
    {
        var (messageIdsToResend, receivedIds) = _packetAcks.ReadIncomingAcks(ackMessageId, acks);

        foreach (var receivedId in receivedIds)
        {
            _pendingReliableMessages.Remove(receivedId);
        }

        if (messageIdsToResend.Count == 0)
        {
            return;
        }

        foreach (var messageId in messageIdsToResend)
        {
            if (_pendingReliableMessages.TryGetValue(messageId, out var message))
            {
                ResendPacket(message.channelId, message.data);
            }
        }
    }

    private void ResendPacket(byte channelId, byte[] data)
    {
        var writer = new LLNetworkWriter();

        WritePacketHeader(writer);

        writer.Write(channelId);
        writer.Write(data);

        Send(writer);
    }

    internal void SendAcks()
    {
        for (var i = 0; i < 3; i++)
        {
            var writer = new LLNetworkWriter();
            BuildPacket(writer, 0, null);
            Send(writer);
        }
    }

    internal ushort NextPacketId()
    {
        return _nextPacketId++;
    }

    internal ushort NextMessageId()
    {
        return _nextMessageId++;
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

    private void WritePacketHeader(LLNetworkWriter writer)
    {
        writer.Write(_connectionId);
        writer.Write(NextPacketId());
        writer.Write(_sessionId);

        var (lastReceivedMessageId, acks) = _packetAcks.GetAcks();
        writer.Write(lastReceivedMessageId);
        writer.Write(acks[0]);

        if (_server.Config.IsAcksLong)
        {
            writer.Write(acks[1]);
        }
    }

    private void BuildPacket(LLNetworkWriter writer, byte channelId, byte[]? data)
    {
        WritePacketHeader(writer);

        if (data == null || data.Length == 0)
        {
            return;
        }

        writer.Write(channelId);

        _channels[channelId].Prepare(writer, data);
    }

    internal void SendPing(PingPacket? incomingPing = null)
    {
        var packet = new PingPacket
        {
            ConnectionId = 0,
            RequestType = SystemPacket.SystemRequestType.Ping,
            PacketId = NextPacketId(),
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
        Handshake,
        Connected,
        Disconnected
    }
}