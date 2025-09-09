namespace UNetLib.LLAPI.Packet;

public class DisconnectPacket : SystemPacket
{
    public uint LibVersion { get; set; }
    public DisconnectReason Reason { get; set; }

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(LibVersion);
        writer.Write(Reason);
    }

    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        LibVersion = reader.ReadUInt32();
        Reason = reader.ReadEnum<DisconnectReason>();
    }

    public override string ToString()
    {
        return $"{base.ToString()}, LibVersion={LibVersion}, Reason={Reason}";
    }

    public enum DisconnectReason : byte
    {
        Ok,
        WrongHost,
        WrongConnection,
        WrongChannel,
        NoResources,
        BadMessage,
        Timeout,
        MessageToLong,
        WrongOperation,
        VersionMismatch,
        CRCMismatch,
        DNSFailure
    }
}