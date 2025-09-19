namespace UNetLib.LLAPI.Packet;

public abstract class SystemPacket : PacketBaseHeader
{
    public SystemRequestType RequestType { get; set; }
    /// <summary>
    /// The packet ID.
    /// This is incremented by 1 for each packet sent on a connection.
    /// Both client and server maintain their own packet ID counters.
    /// </summary>
    public ushort PacketId { get; set; }
    /// <summary>
    /// This is a random number that is different on both client and server.
    /// </summary>
    public ushort SessionId { get; set; }
    /// <summary>
    /// <para>
    /// The local connection ID.
    /// This connection ID is different on the client and server.
    /// </para>
    /// <para>On clientbound packets, it is the ID assigned to the client by the server.</para>
    /// <para>On serverbound packets, it appears to always be 1.</para>
    /// </summary>
    public ushort LocalConnectionId { get; set; }
    /// <summary>
    /// <para>
    /// The remote connection ID.
    /// This connection ID is different on the client and server.
    /// </para>
    /// <para>On clientbound packets, it appears to always be 1.</para>
    /// <para>
    /// On serverbound packets, it is the ID assigned to the client by the server.
    /// If the server has not assigned an ID to the client yet, this will be 0.
    /// </para>
    /// </summary>
    public ushort RemoteConnectionId { get; set; }

    public override void Serialize(LLNetworkWriter writer)
    {
        writer.Write(ConnectionId);
        writer.Write(RequestType);
        writer.Write(PacketId);
        writer.Write(SessionId);
        writer.Write(LocalConnectionId);
        writer.Write(RemoteConnectionId);
    }

    public override void Deserialize(LLNetworkReader reader)
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