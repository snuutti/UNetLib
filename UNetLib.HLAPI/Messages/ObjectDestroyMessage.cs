namespace UNetLib.HLAPI.Messages;

public class ObjectDestroyMessage : IMessageBase
{
    public NetworkInstanceId NetId { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        NetId = reader.ReadNetworkId();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(NetId);
    }

    public override string ToString()
    {
        return $"ObjectDestroyMessage(NetId={NetId})";
    }
}