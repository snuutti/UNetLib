namespace UNetLib.LLAPI;

public class ConnectionConfig
{
    public bool IsAcksLong { get; set; }
    public List<QosType> Channels { get; set; } = [];

    public QosType GetChannelType(byte channelId)
    {
        if (channelId >= Channels.Count)
        {
            return QosType.Unreliable;
        }

        return Channels[channelId];
    }
}