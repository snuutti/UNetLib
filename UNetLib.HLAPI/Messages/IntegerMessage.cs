namespace UNetLib.HLAPI.Messages;

public class IntegerMessage : IMessageBase
{
    public int Value { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        Value = (int) reader.ReadPackedUInt32();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WritePackedUInt32((uint) Value);
    }

    public override string ToString()
    {
        return $"IntegerMessage(Value={Value})";
    }
}