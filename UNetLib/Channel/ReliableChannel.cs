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

        if (!Client.PacketAcks.ReceiveMessage(messageId))
        {
            return;
        }

        Client.SendAcks();

        var payloadLength = length - 5;
        ReadPayload(reader, payloadLength);
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var length = data.Length + 5;
        writer.Write((ushort) length);
        writer.Write(Client.NextMessageId());
        writer.Write(data);
    }
}