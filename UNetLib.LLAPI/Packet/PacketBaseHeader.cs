namespace UNetLib.LLAPI.Packet;

public abstract class PacketBaseHeader
{
    /// <summary>
    /// If 0, this is a system packet.
    /// Otherwise, identical to <see cref="SystemPacket.LocalConnectionId"/>.
    /// </summary>
    public ushort ConnectionId { get; set; }

    public abstract void Serialize(LLNetworkWriter writer);
    public abstract void Deserialize(LLNetworkReader reader);
}