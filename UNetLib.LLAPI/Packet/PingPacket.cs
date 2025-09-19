namespace UNetLib.LLAPI.Packet;

/// <summary>
/// Sent by either the client or server to measure latency and packet loss.
/// Also serves as a connection request acknowledgment.
/// </summary>
public class PingPacket : SystemPacket
{
    /// <summary>
    /// The time this packet was sent in milliseconds since the application started.
    /// </summary>
    public uint SentPingTime { get; set; }
    /// <summary>
    /// The time the last ping packet was received from the remote host.
    /// Copied from the last received ping packet.
    /// </summary>
    public uint AckPingTime { get; set; }
    /// <summary>
    /// How many milliseconds has elapsed since the last ping packet was received and when this packet was sent.
    /// </summary>
    public uint LocalTimeCorrection { get; set; }
    /// <summary>
    /// How many packets were dropped due to lack of internal buffer space.
    /// </summary>
    public byte IntDropRate { get; set; }
    /// <summary>
    /// How many packets were lost or delivered out of order.
    /// </summary>
    public byte ExtDropRate { get; set; }
    /// <summary>
    /// This is a random ping specific session ID that is different on both client and server.
    /// </summary>
    public ushort RemoteSessionId { get; set; }

    public override void Serialize(LLNetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(SentPingTime);
        writer.Write(AckPingTime);
        writer.Write(LocalTimeCorrection);
        writer.Write(IntDropRate);
        writer.Write(ExtDropRate);
        writer.Write(RemoteSessionId);
    }

    public override void Deserialize(LLNetworkReader reader)
    {
        base.Deserialize(reader);
        SentPingTime = reader.ReadUInt32();
        AckPingTime = reader.ReadUInt32();
        LocalTimeCorrection = reader.ReadUInt32();
        IntDropRate = reader.ReadByte();
        ExtDropRate = reader.ReadByte();
        RemoteSessionId = reader.ReadUInt16();
    }

    public override string ToString()
    {
        return $"{base.ToString()}, SentPingTime={SentPingTime}, AckPingTime={AckPingTime}, LocalTimeCorrection={LocalTimeCorrection}, IntDropRate={IntDropRate}, ExtDropRate={ExtDropRate}, RemoteSessionId={RemoteSessionId}";
    }
}