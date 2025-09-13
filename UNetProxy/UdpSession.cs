using System.Net;
using System.Net.Sockets;
using Spectre.Console;
using UNetLib.LLAPI;
using UNetLib.LLAPI.Packet;

namespace UNetProxy;

public sealed class UdpSession : IDisposable
{
    private readonly IPEndPoint _clientEndPoint;
    private readonly IPEndPoint _targetEndPoint;
    private readonly UdpClient _clientListener;
    private readonly UdpClient _forwardingClient;
    private readonly CancellationTokenSource _cts;
    private readonly ProxySettings _settings;

    public UdpSession(IPEndPoint clientEndPoint, IPEndPoint targetEndPoint, UdpClient clientListener,
        CancellationTokenSource parentCts, ProxySettings settings)
    {
        _clientEndPoint = clientEndPoint;
        _targetEndPoint = targetEndPoint;
        _clientListener = clientListener;
        _forwardingClient = new UdpClient();
        _forwardingClient.Connect(targetEndPoint);
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
            AnsiConsole.MarkupLine($"[red]{e}[/]");
        }
    }

    public async Task OnPacketReceived(byte[] buffer, bool fromClient)
    {
        if (fromClient)
        {
            LogPacket(buffer, _clientEndPoint, _targetEndPoint, true);
            await _forwardingClient.SendAsync(buffer, buffer.Length);
        }
        else
        {
            LogPacket(buffer, _targetEndPoint, _clientEndPoint, false);
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

        var reader = new NetworkReader(buffer);
        var connectionId = reader.ReadUInt16();
        if (connectionId != 0)
        {
            var packetId = reader.ReadUInt16();
            var sessionId = reader.ReadUInt16();

            var ackMessageId = reader.ReadUInt16();
            var acks = new uint[1];
            acks[0] = reader.ReadUInt32();

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

            // TODO: Implement user packet handling
            AnsiConsole.MarkupLine($"[green][[{DateTime.Now:HH:mm:ss}]] {direction} (User Packet) From {from} To {to} ({buffer.Length} bytes)[/]\n{BitConverter.ToString(buffer)}");
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
}