namespace UNetLib.LLAPI.Utils;

public static class ChannelUtils
{
    public static bool IsChannelReliable(QosType qosType)
    {
        return qosType is QosType.Reliable or QosType.ReliableFragmented or QosType.ReliableSequenced
            or QosType.ReliableStateUpdate or QosType.AllCostDelivery;
    }

    public static bool IsChannelSequenced(QosType qosType)
    {
        return qosType is QosType.UnreliableSequenced or QosType.ReliableSequenced;
    }
}