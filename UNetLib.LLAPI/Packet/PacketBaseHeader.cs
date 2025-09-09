namespace UNetLib.LLAPI.Packet;

public abstract class PacketBaseHeader
{
    public ushort ConnectionId { get; set; }

    public abstract void Serialize(NetworkWriter writer);
    public abstract void Deserialize(NetworkReader reader);
}