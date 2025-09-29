namespace UNetLib.LLAPI.Utils;

public static class ConnectionUtils
{
    public static ushort CalculateRemoteSessionId(ushort clientSessionId)
    {
        var highByte = clientSessionId >> 8;
        var lowByte = (clientSessionId & 0x00FF) << 8;
        return (ushort) (lowByte | highByte);
    }
}