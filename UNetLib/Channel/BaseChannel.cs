using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal abstract class BaseChannel
{
    protected readonly UNetClient Client;

    protected readonly byte ChannelId;

    protected BaseChannel(UNetClient client, byte channelId)
    {
        Client = client;
        ChannelId = channelId;
    }

    public abstract void Process(LLNetworkReader reader);

    public abstract void Prepare(LLNetworkWriter writer, byte[] data);

    public virtual void Send(byte[] data)
    {
        var writer = new LLNetworkWriter();
        Client.BuildPacket(writer, ChannelId, data);
        Client.Send(writer);
    }

    protected void SkipPayload(LLNetworkReader reader, int length)
    {
        if (length < 0 || reader.Position + length > reader.Length)
        {
            Console.WriteLine($"Invalid payload length {length} from {Client.RemoteEndPoint}!");
            return;
        }

        reader.ReadBytes(length);
    }

    protected void ReadPayload(LLNetworkReader reader, int length)
    {
        if (length < 0 || reader.Position + length > reader.Length)
        {
            Console.WriteLine($"Invalid payload length {length} from {Client.RemoteEndPoint}!");
            return;
        }

        var payload = reader.ReadBytes(length);
        ReadPayload(payload);
    }

    protected void ReadPayload(byte[] payload)
    {
        Client.EventListener.OnNetworkReceive(Client, new ArraySegment<byte>(payload), ChannelId);
    }
}