namespace UNetLib.LLAPI.Packet;

public abstract class SystemPacket : PacketBaseHeader
{
    public SystemRequestType RequestType { get; set; }
    public ushort PacketId { get; set; }
    public ushort SessionId { get; set; }
    public ushort LocalConnectionId { get; set; }
    public ushort RemoteConnectionId { get; set; }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(ConnectionId);
        writer.Write(RequestType);
        writer.Write(PacketId);
        writer.Write(SessionId);
        writer.Write(LocalConnectionId);
        writer.Write(RemoteConnectionId);
    }

    public override void Deserialize(NetworkReader reader)
    {
        ConnectionId = reader.ReadUInt16();
        RequestType = reader.ReadEnum<SystemRequestType>();
        PacketId = reader.ReadUInt16();
        SessionId = reader.ReadUInt16();
        LocalConnectionId = reader.ReadUInt16();
        RemoteConnectionId = reader.ReadUInt16();
    }

    public override string ToString()
    {
        return $"ConnectionId={ConnectionId}, RequestType={RequestType}, PacketId={PacketId}, SessionId={SessionId}, LocalConnectionId={LocalConnectionId}, RemoteConnectionId={RemoteConnectionId}";
    }

    public enum SystemRequestType : byte
    {
        ConnectRequest = 1,
        Disconnect = 3,
        Ping = 4
    }
}