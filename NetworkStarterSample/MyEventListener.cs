using UNetLib;
using UNetLib.LLAPI.Packet;

namespace NetworkStarterSample;

public class MyEventListener : IUNetEventListener
{
    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.Accept();
    }

    public void OnClientConnected(UNetClient client)
    {
        Console.WriteLine($"Client connected: {client.RemoteEndPoint}");
    }

    public void OnClientDisconnected(UNetClient client, DisconnectPacket.DisconnectReason reason)
    {
        Console.WriteLine($"Client disconnected: {client.RemoteEndPoint}, Reason: {reason}");
    }

    public void OnNetworkReceive(UNetClient client, ArraySegment<byte> data, byte channelId)
    {
        Console.WriteLine($"Received data from {client.RemoteEndPoint} on channel {channelId}, Length: {data.Count}");
    }
}