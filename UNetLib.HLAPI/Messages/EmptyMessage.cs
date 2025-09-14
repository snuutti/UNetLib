namespace UNetLib.HLAPI.Messages;

public class EmptyMessage : IMessageBase
{
    public void Deserialize(NetworkReader reader)
    {
    }

    public void Serialize(NetworkWriter writer)
    {
    }

    public override string ToString()
    {
        return "EmptyMessage";
    }
}