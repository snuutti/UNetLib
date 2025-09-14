namespace UNetLib.LLAPI;

public class ConnectionConfig
{
    public bool IsAcksLong { get; set; }
    public List<QosType> Channels { get; set; } = [];
}