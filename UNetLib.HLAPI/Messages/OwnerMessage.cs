namespace UNetLib.HLAPI.Messages;

public class OwnerMessage : IMessageBase
{
    public NetworkInstanceId NetId { get; set; }
    public short PlayerControllerId { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        NetId = reader.ReadNetworkId();
        PlayerControllerId = (short) reader.ReadPackedUInt32();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(NetId);
        writer.WritePackedUInt32((uint) PlayerControllerId);
    }

    public override string ToString()
    {
        return $"OwnerMessage(NetId={NetId}, PlayerControllerId={PlayerControllerId})";
    }
}