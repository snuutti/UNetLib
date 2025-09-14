namespace UNetLib.LLAPI.Packet;

public abstract class PacketBaseHeader
{
    public ushort ConnectionId { get; set; }

    public abstract void Serialize(LLNetworkWriter writer);
    public abstract void Deserialize(LLNetworkReader reader);
}