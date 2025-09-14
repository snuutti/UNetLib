namespace UNetLib.HLAPI.Messages;

public class RemovePlayerMessage : IMessageBase
{
    public short PlayerControllerId { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        PlayerControllerId = (short) reader.ReadUInt16();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write((ushort) PlayerControllerId);
    }

    public override string ToString()
    {
        return $"RemovePlayerMessage(PlayerControllerId={PlayerControllerId})";
    }
}