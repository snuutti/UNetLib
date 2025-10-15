using System.Buffers;
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

    private CancellationTokenSource? _cts;

    private readonly ConcurrentDictionary<IPEndPoint, UNetClient> _clients = new();

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
        _cts = new CancellationTokenSource();

        Task.Run(ReceiveLoop, _cts.Token);

        IsRunning = true;
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        _cts?.Cancel();
        _udpClient?.Close();
        IsRunning = false;
    }

    private async Task ReceiveLoop()
    {
        var token = _cts!.Token;
        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = await _udpClient!.ReceiveAsync(token);

                var length = result.Buffer.Length;
                var buffer = ArrayPool<byte>.Shared.Rent(length);
                Buffer.BlockCopy(result.Buffer, 0, buffer, 0, length);

                ThreadPool.QueueUserWorkItem(ProcessPacketWorkItem,
                    new PacketWorkItem(buffer, length, result.RemoteEndPoint), false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex}");
            }
        }
    }

    private void ProcessPacketWorkItem(PacketWorkItem item)
    {
        try
        {
            var buffer = item.Buffer.AsSpan(0, item.Length);
            HandleIncomingPacket(buffer, item.RemoteEndPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing packet from {item.RemoteEndPoint}: {ex}");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(item.Buffer);
        }
    }

    private void HandleIncomingPacket(Span<byte> buffer, IPEndPoint remoteEndPoint)
    {
        if (buffer.Length < 2)
        {
            return;
        }

        var reader = new LLNetworkReader(buffer.ToArray());
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

        if (!_clients.TryGetValue(remoteEndPoint, out var client) &&
            requestType != SystemPacket.SystemRequestType.ConnectRequest)
        {
            return;
        }

        switch (requestType)
        {
            case SystemPacket.SystemRequestType.ConnectRequest:
                var connectRequest = new ConnectPacket();
                connectRequest.Deserialize(reader);
                HandleConnectRequest(connectRequest, remoteEndPoint);
                break;

            case SystemPacket.SystemRequestType.Disconnect:
                var disconnectPacket = new DisconnectPacket();
                disconnectPacket.Deserialize(reader);
                RemoveClient(client!, disconnectPacket.Reason);
                break;

            case SystemPacket.SystemRequestType.Ping:
                var pingPacket = new PingPacket();
                pingPacket.Deserialize(reader);
                client!.ProcessPing(pingPacket);
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

        // TODO: We need to manage connection IDs properly. With the current approach they will eventually wrap around and cause issues.
        var newConnectionId = (ushort) Interlocked.Increment(ref _nextConnectionId);
        var newSessionId = (ushort) Random.Shared.Next(1, ushort.MaxValue);

        var client = new UNetClient(this, remoteEndPoint, newConnectionId, packet.LocalConnectionId, newSessionId, packet.SessionId);
        _clients[remoteEndPoint] = client;

        client.SendPing();
        _eventListener.OnClientConnected(client);
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
        _eventListener.OnClientDisconnected(client, reason);
    }

    public void Send(IPEndPoint remoteEndPoint, byte[] data)
    {
        _udpClient?.Send(data, data.Length, remoteEndPoint);
    }
}