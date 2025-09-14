namespace UNetLib.HLAPI.Messages;

public class StringMessage : IMessageBase
{
    public string Value { get; set; } = string.Empty;

    public void Deserialize(NetworkReader reader)
    {
        Value = reader.ReadString();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(Value);
    }

    public override string ToString()
    {
        return $"StringMessage(Value='{Value}')";
    }
}