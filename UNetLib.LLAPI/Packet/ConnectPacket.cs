namespace UNetLib.LLAPI.Packet;

public class ConnectPacket : SystemPacket
{
    public uint LibVersion { get; set; }
    public uint Crc { get; set; }

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(LibVersion);
        writer.Write(Crc);
    }

    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        LibVersion = reader.ReadUInt32();
        Crc = reader.ReadUInt32();
    }

    public override string ToString()
    {
        return $"{base.ToString()}, LibVersion={LibVersion}, Crc={Crc}";
    }
}