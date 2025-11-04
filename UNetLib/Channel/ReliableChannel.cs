using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal sealed class ReliableChannel : BaseChannel
{
    public ReliableChannel(UNetClient client, byte channelId) : base(client, channelId)
    {
    }

    public override void Process(LLNetworkReader reader)
    {
        var length = reader.ReadUInt16();
        var messageId = reader.ReadUInt16();

        var payloadLength = length - 5;
        if (!Client.PacketAcks.ReceiveMessage(messageId))
        {
            SkipPayload(reader, payloadLength);
            return;
        }

        Client.SendAcks();

        ReadPayload(reader, payloadLength);
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var messageId = Client.NextMessageId();

        var payloadWriter = new LLNetworkWriter();

        var length = data.Length + 5;
        payloadWriter.Write((ushort) length);
        payloadWriter.Write(messageId);
        payloadWriter.Write(data);

        var payload = payloadWriter.ToArray();
        writer.Write(payload);

        Client.StoreReliableMessage(messageId, ChannelId, payload);
    }
}