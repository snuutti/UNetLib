namespace UNetLib.HLAPI.Messages;

public class ErrorMessage : IMessageBase
{
    public ushort ErrorCode { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        ErrorCode = reader.ReadUInt16();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(ErrorCode);
    }

    public override string ToString()
    {
        return $"ErrorMessage(ErrorCode={ErrorCode})";
    }
}