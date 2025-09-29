using System.Net;

namespace UNetLib.LLAPI;

internal readonly struct PacketWorkItem
{
    public readonly byte[] Buffer;
    public readonly int Length;
    public readonly IPEndPoint RemoteEndPoint;

    public PacketWorkItem(byte[] buffer, int length, IPEndPoint remoteEndPoint)
    {
        Buffer = buffer;
        Length = length;
        RemoteEndPoint = remoteEndPoint;
    }
}