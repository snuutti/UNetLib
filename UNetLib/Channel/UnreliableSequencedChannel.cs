using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal sealed class UnreliableSequencedChannel : BaseChannel
{
    private byte _lastOrderedMessageId;

    public UnreliableSequencedChannel(UNetClient client, byte channelId) : base(client, channelId)
    {
    }

    public override void Process(LLNetworkReader reader)
    {
        var length = reader.ReadUInt16();
        var orderedMessageId = reader.ReadByte();

        // Drop the packet if it's older than the last processed one
        if (orderedMessageId <= _lastOrderedMessageId && _lastOrderedMessageId != 255)
        {
            return;
        }

        _lastOrderedMessageId = orderedMessageId;

        var payloadLength = length - 4;
        ReadPayload(reader, payloadLength);
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var length = data.Length + 4;
        writer.Write((ushort) length);
        writer.Write(_lastOrderedMessageId++);
        writer.Write(data);
    }
}