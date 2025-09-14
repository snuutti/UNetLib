namespace UNetLib.HLAPI.Messages;

public class ObjectSpawnFinishedMessage : IMessageBase
{
    public uint State { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        State = reader.ReadPackedUInt32();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WritePackedUInt32(State);
    }

    public override string ToString()
    {
        return $"ObjectSpawnFinishedMessage(State={State})";
    }
}