namespace UNetLib.LLAPI.Packet;

public class PingPacket : SystemPacket
{
    public uint SentPingTime { get; set; }
    public uint AckPingTime { get; set; }
    public uint LocalTimeCorrection { get; set; }
    public byte IntDropRate { get; set; }
    public byte ExtDropRate { get; set; }
    public ushort RemoteSessionId { get; set; }

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(SentPingTime);
        writer.Write(AckPingTime);
        writer.Write(LocalTimeCorrection);
        writer.Write(IntDropRate);
        writer.Write(ExtDropRate);
        writer.Write(RemoteSessionId);
    }

    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        SentPingTime = reader.ReadUInt32();
        AckPingTime = reader.ReadUInt32();
        LocalTimeCorrection = reader.ReadUInt32();
        IntDropRate = reader.ReadByte();
        ExtDropRate = reader.ReadByte();
        RemoteSessionId = reader.ReadUInt16();
    }
}