using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UNetLib.LLAPI;
using UNetLib.LLAPI.Packet;

namespace UNetLib;

// TODO: Implement timeouts
public class UNetServer
{
    private readonly ConnectionConfig _config;

    private readonly IUNetEventListener _eventListener;

    private UdpClient? _udpClient;

    private readonly ConcurrentDictionary<IPEndPoint, UNetClient> _clients = new();

    private readonly ConcurrentQueue<ushort> _connectionIds = new();

    private IPEndPoint _remoteEndPoint = new(IPAddress.Any, 0);

    private int _nextConnectionId;

    public bool IsRunning { get; private set; }

    public ConnectionConfig Config => _config;

    internal IUNetEventListener EventListener => _eventListener;

    public UNetServer(ConnectionConfig config, IUNetEventListener eventListener)
    {
        _config = config;
        _eventListener = eventListener;
    }

    public void Start(int port)
    {
        if (IsRunning)
        {
            return;
        }

        _udpClient = new UdpClient(port);
        IsRunning = true;
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        _udpClient?.Close();
        IsRunning = false;
    }

    public void Update()
    {
        PollEvents();
    }

    private void PollEvents()
    {
        if (!IsRunning || _udpClient == null)
        {
            return;
        }

        while (_udpClient.Available > 0)
        {
            try
            {
                var result = _udpClient.Receive(ref _remoteEndPoint);

                try
                {
                    HandleIncomingPacket(result, _remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing packet from {_remoteEndPoint}: {ex}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex}");
            }
        }
    }

    private void HandleIncomingPacket(byte[] buffer, IPEndPoint remoteEndPoint)
    {
        if (buffer.Length < 2)
        {
            return;
        }

        var reader = new LLNetworkReader(buffer);
        var connectionId = reader.ReadUInt16();

        if (connectionId == 0)
        {
            HandleSystemPacket(reader, remoteEndPoint);
        }
        else
        {
            HandleDataPacket(reader, connectionId, remoteEndPoint);
        }
    }

    private void HandleSystemPacket(LLNetworkReader reader, IPEndPoint remoteEndPoint)
    {
        var requestType = reader.ReadEnum<SystemPacket.SystemRequestType>();
        reader.Position = 0;

        if (requestType == SystemPacket.SystemRequestType.ConnectRequest)
        {
            var connectRequestPacket = new ConnectPacket();
            connectRequestPacket.Deserialize(reader);
            HandleConnectRequest(connectRequestPacket, remoteEndPoint);
            return;
        }

        if (!_clients.TryGetValue(remoteEndPoint, out var client))
        {
            return;
        }

        switch (requestType)
        {
            case SystemPacket.SystemRequestType.Disconnect:
                var disconnectPacket = new DisconnectPacket();
                disconnectPacket.Deserialize(reader);
                RemoveClient(client, disconnectPacket.Reason);
                break;

            case SystemPacket.SystemRequestType.Ping:
                var pingPacket = new PingPacket();
                pingPacket.Deserialize(reader);
                client.ProcessPing(pingPacket);
                break;
        }
    }

    private void HandleDataPacket(LLNetworkReader reader, ushort connectionId, IPEndPoint remoteEndPoint)
    {
        if (!_clients.TryGetValue(remoteEndPoint, out var client) || client.ConnectionId != connectionId)
        {
            return;
        }

        var packetId = reader.ReadUInt16();
        var sessionId = reader.ReadUInt16();

        var ackMessageId = reader.ReadUInt16();
        uint[] acks;

        if (_config.IsAcksLong)
        {
            acks = new uint[2];
            acks[0] = reader.ReadUInt32();
            acks[1] = reader.ReadUInt32();
        }
        else
        {
            acks = new uint[1];
            acks[0] = reader.ReadUInt32();
        }

        client.ProcessAcks(ackMessageId, acks);

        if (reader.IsAtEnd)
        {
            // Ack only packet
            return;
        }

        client.ProcessDataPacket(reader);
    }

    private void HandleConnectRequest(ConnectPacket packet, IPEndPoint remoteEndPoint)
    {
        if (_clients.ContainsKey(remoteEndPoint))
        {
            return;
        }

        var request = new ConnectionRequest(this, packet, remoteEndPoint);
        _eventListener.OnConnectionRequest(request);
    }

    internal void AcceptConnection(IPEndPoint remoteEndPoint, ConnectPacket packet)
    {
        var newConnectionId = GetNextConnectionId();
        var newSessionId = (ushort) Random.Shared.Next(1, ushort.MaxValue);

        var client = new UNetClient(this, remoteEndPoint, newConnectionId, packet.LocalConnectionId, newSessionId, packet.SessionId);
        _clients[remoteEndPoint] = client;

        client.SendPing();
    }

    internal void DenyConnection(IPEndPoint remoteEndPoint, ConnectPacket packet, DisconnectPacket.DisconnectReason reason)
    {
        var disconnectPacket = new DisconnectPacket
        {
            ConnectionId = 0,
            RequestType = SystemPacket.SystemRequestType.Disconnect,
            PacketId = 0,
            SessionId = packet.SessionId,
            LocalConnectionId = 0,
            RemoteConnectionId = packet.LocalConnectionId,
            LibVersion = 16777472,
            Reason = reason
        };

        var writer = new LLNetworkWriter();
        disconnectPacket.Serialize(writer);
        Send(remoteEndPoint, writer.ToArray());
    }

    private ushort GetNextConnectionId()
    {
        if (_connectionIds.TryDequeue(out var id))
        {
            return id;
        }

        return (ushort) Interlocked.Increment(ref _nextConnectionId);
    }

    public void Disconnect(UNetClient client, DisconnectPacket.DisconnectReason reason)
    {
        client.SendDisconnect(reason);
        RemoveClient(client, reason);
    }

    private void RemoveClient(UNetClient client, DisconnectPacket.DisconnectReason reason)
    {
        if (client.State == UNetClient.ConnectionState.Disconnected)
        {
            return;
        }

        client.State = UNetClient.ConnectionState.Disconnected;

        _clients.TryRemove(client.RemoteEndPoint, out _);
        _connectionIds.Enqueue(client.ConnectionId);
        _eventListener.OnClientDisconnected(client, reason);
    }

    public void Send(IPEndPoint remoteEndPoint, byte[] data)
    {
        _udpClient?.Send(data, data.Length, remoteEndPoint);
    }
}