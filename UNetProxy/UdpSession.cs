using System.Net;
using System.Net.Sockets;
using Spectre.Console;
using UNetLib.HLAPI;
using UNetLib.HLAPI.Messages;
using UNetLib.LLAPI;
using UNetLib.LLAPI.Packet;

namespace UNetProxy;

public sealed class UdpSession : IDisposable
{
    private readonly IPEndPoint _clientEndPoint;
    private readonly IPEndPoint _targetEndPoint;
    private readonly UdpClient _clientListener;
    private readonly UdpClient _forwardingClient;
    private readonly ConnectionConfig _connectionConfig;
    private readonly CancellationTokenSource _cts;
    private readonly ProxySettings _settings;

    public UdpSession(IPEndPoint clientEndPoint, IPEndPoint targetEndPoint, UdpClient clientListener,
        ConnectionConfig connectionConfig, CancellationTokenSource parentCts, ProxySettings settings)
    {
        _clientEndPoint = clientEndPoint;
        _targetEndPoint = targetEndPoint;
        _clientListener = clientListener;
        _forwardingClient = new UdpClient();
        _forwardingClient.Connect(targetEndPoint);
        _connectionConfig = connectionConfig;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(parentCts.Token);
        _settings = settings;

        _ = ReceiveLoopAsync();
    }

    private async Task ReceiveLoopAsync()
    {
        var ct = _cts.Token;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var result = await _forwardingClient.ReceiveAsync(ct);
                await OnPacketReceived(result.Buffer, false);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
    }

    public async Task OnPacketReceived(byte[] buffer, bool fromClient)
    {
        if (fromClient)
        {
            try
            {
                LogPacket(buffer, _clientEndPoint, _targetEndPoint, true);
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }

            await _forwardingClient.SendAsync(buffer, buffer.Length);
        }
        else
        {
            try
            {
                LogPacket(buffer, _targetEndPoint, _clientEndPoint, false);
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }

            await _clientListener.SendAsync(buffer, buffer.Length, _clientEndPoint);
        }
    }

    public void Dispose()
    {
        _cts.Dispose();
        _forwardingClient.Dispose();
    }

    private void LogPacket(byte[] buffer, IPEndPoint from, IPEndPoint to, bool fromClient)
    {
        var direction = fromClient ? "[[CLIENT -> SERVER]]" : "[[SERVER -> CLIENT]]";

        var reader = new LLNetworkReader(buffer);
        var connectionId = reader.ReadUInt16();
        if (connectionId != 0)
        {
            var packetId = reader.ReadUInt16();
            var sessionId = reader.ReadUInt16();

            var ackMessageId = reader.ReadUInt16();
            uint[] acks;

            if (_connectionConfig.IsAcksLong)
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

            var ackPacket = $"ConnectionId={connectionId}, PacketId={packetId}, SessionId={sessionId}, AckMessageId={ackMessageId}, Acks={string.Join(", ", acks)}";
            if (reader.IsAtEnd)
            {
                if (!_settings.LogAcks)
                {
                    return;
                }

                AnsiConsole.MarkupLine($"[green][[{DateTime.Now:HH:mm:ss}]] {direction} (Acknowledge Packet) From {from} To {to} ({buffer.Length} bytes)[/]\n{ackPacket}");
                return;
            }

            var channelId = reader.ReadByte();
            var qosType = GetChannelType(channelId);
            var length = reader.ReadUInt16();

            var messageId = reader.ReadUInt16();

            if (qosType is QosType.UnreliableSequenced or QosType.ReliableSequenced)
            {
                var orderedMessageId = reader.ReadByte();
            }

            var payloadLength = length - 6;
            var payload = reader.ReadBytes(payloadLength);

            AnsiConsole.MarkupLine($"[green][[{DateTime.Now:HH:mm:ss}]] {direction} (User Packet) From {from} To {to} ({buffer.Length} bytes)[/]\n{ackPacket}");

            var hlapiReader = new NetworkReader(payload);

            while (hlapiReader.Position < payloadLength)
            {
                var size = hlapiReader.ReadUInt16();
                var msgType = hlapiReader.ReadInt16();
                var msgBuffer = hlapiReader.ReadBytes(size);

                IMessageBase message;
                switch (msgType)
                {
                    case 1:// ObjectDestroy
                        message = new ObjectDestroyMessage();
                        break;

                    case 3:// ObjectSpawn
                        message = new ObjectSpawnMessage();
                        break;

                    case 4:// Owner
                        message = new OwnerMessage();
                        break;

                    case 12:// SpawnFinished
                        message = new ObjectSpawnFinishedMessage();
                        break;

                    case 14:// CRC
                        message = new CrcMessage();
                        break;

                    case 35:// Ready
                        message = new ReadyMessage();
                        break;

                    case 36:// NotReady
                        message = new NotReadyMessage();
                        break;

                    case 37:// AddPlayer
                        message = new AddPlayerMessage();
                        break;

                    case 38:// RemovePlayer
                        message = new RemovePlayerMessage();
                        break;

                    case 39:// Scene
                        message = new StringMessage();
                        break;

                    case 43:// LobbyReadyToBegin
                        message = new LobbyReadyToBeginMessage();
                        break;

                    case 44:// LobbySceneLoaded
                        message = new IntegerMessage();
                        break;

                    case 45:// LobbyAddPlayerFailed
                        message = new EmptyMessage();
                        break;

                    default:
                        AnsiConsole.WriteLine($"  HLAPI Message: Size={size}, MsgType={msgType}, MsgBuffer={BitConverter.ToString(msgBuffer)}");
                        continue;
                }

                var msgReader = new NetworkReader(msgBuffer);
                message.Deserialize(msgReader);
                AnsiConsole.WriteLine($"  HLAPI Message: {message}");
            }

            return;
        }

        SystemPacket systemPacket;
        var requestType = reader.ReadEnum<SystemPacket.SystemRequestType>();
        reader.Position = 0;
        switch (requestType)
        {
            case SystemPacket.SystemRequestType.ConnectRequest:
                systemPacket = new ConnectPacket();
                break;

            case SystemPacket.SystemRequestType.Disconnect:
                systemPacket = new DisconnectPacket();
                break;

            case SystemPacket.SystemRequestType.Ping:
                if (!_settings.LogPings)
                {
                    return;
                }

                systemPacket = new PingPacket();
                break;

            default:
                AnsiConsole.MarkupLine($"[red][[{DateTime.Now:HH:mm:ss}]] {direction} (Unknown System Packet: {requestType}) From {from} To {to} ({buffer.Length} bytes)[/]\n{BitConverter.ToString(buffer)}");
                return;
        }

        systemPacket.Deserialize(reader);
        AnsiConsole.MarkupLine($"[blue][[{DateTime.Now:HH:mm:ss}]] {direction} ({requestType}) From {from} To {to} ({buffer.Length} bytes)[/]\n{systemPacket}");
    }

    private QosType GetChannelType(byte channelId)
    {
        if (channelId >= _connectionConfig.Channels.Count)
        {
            return QosType.Unreliable;
        }

        return _connectionConfig.Channels[channelId];
    }
}