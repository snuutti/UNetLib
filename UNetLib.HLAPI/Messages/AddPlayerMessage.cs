namespace UNetLib.HLAPI.Messages;

public class AddPlayerMessage : IMessageBase
{
    public short PlayerControllerId { get; set; }
    public int MsgSize { get; set; }
    public byte[]? MsgData { get; set; } = [];

    public void Deserialize(NetworkReader reader)
    {
        PlayerControllerId = (short) reader.ReadUInt16();
        MsgData = reader.ReadBytesAndSize();
        if (MsgData == null)
        {
            MsgSize = 0;
        }
        else
        {
            MsgSize = MsgData.Length;
        }
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write((ushort) PlayerControllerId);
        writer.WriteBytesAndSize(MsgData, MsgSize);
    }

    public override string ToString()
    {
        return $"AddPlayerMessage(PlayerControllerId={PlayerControllerId}, MsgSize={MsgSize})";
    }
}