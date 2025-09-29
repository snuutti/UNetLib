using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal abstract class BaseChannel
{
    protected readonly UNetClient Client;

    private readonly byte _channelId;

    protected BaseChannel(UNetClient client, byte channelId)
    {
        Client = client;
        _channelId = channelId;
    }

    public abstract void Process(LLNetworkReader reader);

    public abstract void Prepare(LLNetworkWriter writer, byte[] data);

    protected void ReadPayload(LLNetworkReader reader, int length)
    {
        if (length < 0 || reader.Position + length > reader.Length)
        {
            Console.WriteLine($"Invalid payload length {length} from {Client.RemoteEndPoint}!");
            return;
        }

        var payload = reader.ReadBytes(length);
        Client.EventListener.OnNetworkReceive(Client, new ArraySegment<byte>(payload), _channelId);
    }
}