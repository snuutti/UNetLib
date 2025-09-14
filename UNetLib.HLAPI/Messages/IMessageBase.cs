namespace UNetLib.HLAPI.Messages;

public interface IMessageBase
{
    public void Deserialize(NetworkReader reader);
    public void Serialize(NetworkWriter writer);
}