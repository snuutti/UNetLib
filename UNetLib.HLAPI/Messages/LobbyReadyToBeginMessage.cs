namespace UNetLib.HLAPI.Messages;

public class LobbyReadyToBeginMessage : IMessageBase
{
    public byte SlotId { get; set; }
    public bool ReadyState { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        SlotId = reader.ReadByte();
        ReadyState = reader.ReadBoolean();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(SlotId);
        writer.Write(ReadyState);
    }

    public override string ToString()
    {
        return $"LobbyReadyToBeginMessage(SlotId={SlotId}, ReadyState={ReadyState})";
    }
}