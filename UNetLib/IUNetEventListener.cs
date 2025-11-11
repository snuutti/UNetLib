using UNetLib.LLAPI.Packet;

namespace UNetLib;

public interface IUNetEventListener
{
    /// <summary>
    /// Called when a new client is requesting a connection.
    /// The connection must be explicitly accepted or rejected via the request object.
    /// </summary>
    /// <param name="request">The connection request object.</param>
    void OnConnectionRequest(ConnectionRequest request);

    /// <summary>
    /// Called when a client has successfully completed the connection handshake.
    /// </summary>
    /// <param name="client">The connected client.</param>
    void OnClientConnected(UNetClient client);

    /// <summary>
    /// Called when a client has disconnected.
    /// </summary>
    /// <param name="client">The disconnected client.</param>
    /// <param name="reason">The reason for the disconnection.</param>
    void OnClientDisconnected(UNetClient client, DisconnectPacket.DisconnectReason reason);

    /// <summary>
    /// Called when data is received from a client.
    /// </summary>
    /// <param name="client">The client that sent the data.</param>
    /// <param name="data">The received data.</param>
    /// <param name="channelId">The channel ID on which the data was received.</param>
    void OnNetworkReceive(UNetClient client, ArraySegment<byte> data, byte channelId);
}