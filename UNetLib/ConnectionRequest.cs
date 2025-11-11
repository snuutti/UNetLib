using System.Net;
using UNetLib.LLAPI.Packet;

namespace UNetLib;

/// <summary>
/// Represents a pending connection request from a remote endpoint.
/// </summary>
public class ConnectionRequest
{
    private readonly UNetServer _server;

    private readonly ConnectPacket _connectPacket;

    private bool _isHandled;

    /// <summary>
    /// The remote endpoint of the client requesting the connection.
    /// </summary>
    public IPEndPoint RemoteEndPoint { get; }

    internal ConnectionRequest(UNetServer server, ConnectPacket connectPacket, IPEndPoint remoteEndPoint)
    {
        _server = server;
        _connectPacket = connectPacket;
        RemoteEndPoint = remoteEndPoint;
    }

    /// <summary>
    /// Accepts the connection request and proceeds with the handshake.
    /// </summary>
    public void Accept()
    {
        if (_isHandled)
        {
            return;
        }

        _isHandled = true;
        _server.AcceptConnection(RemoteEndPoint, _connectPacket);
    }

    /// <summary>
    /// Rejects the connection request and sends a disconnect message to the client.
    /// </summary>
    /// <param name="reason">The reason for rejecting the connection.</param>
    public void Reject(DisconnectPacket.DisconnectReason reason = DisconnectPacket.DisconnectReason.Ok)
    {
        if (_isHandled)
        {
            return;
        }

        _isHandled = true;
        _server.DenyConnection(RemoteEndPoint, _connectPacket, reason);
    }
}