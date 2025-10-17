using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal sealed class ReliableFragmentedChannel : BaseChannel
{
    private const int MaxFragmentPayloadSize = 512;

    private byte _nextFragmentedMessageId;

    private readonly Dictionary<byte, FragmentedMessage> _pendingMessages = new();

    public ReliableFragmentedChannel(UNetClient client, byte channelId) : base(client, channelId)
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

        var fragmentedMessageId = reader.ReadByte();
        var fragmentIndex = reader.ReadByte();
        var fragmentAmount = reader.ReadByte();

        if (!_pendingMessages.TryGetValue(fragmentedMessageId, out var pendingMsg))
        {
            pendingMsg = new FragmentedMessage(fragmentAmount);
            _pendingMessages[fragmentedMessageId] = pendingMsg;
        }

        var payloadLength = length - 8;
        var fragmentData = reader.ReadBytes(payloadLength);

        if (pendingMsg.AddFragment(fragmentIndex, fragmentData))
        {
            var fullPayload = pendingMsg.Reassemble();
            _pendingMessages.Remove(fragmentedMessageId);
            ReadPayload(fullPayload);
        }
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var fragmentedMessageId = _nextFragmentedMessageId++;

        var fragmentAmount = (byte) Math.Ceiling((double) data.Length / MaxFragmentPayloadSize);
        if (fragmentAmount == 0)
        {
            // Handle the case of empty data
            fragmentAmount = 1;
        }

        var dataOffset = 0;
        for (byte i = 0; i < fragmentAmount; i++)
        {
            var remainingData = data.Length - dataOffset;
            var fragmentSize = Math.Min(MaxFragmentPayloadSize, remainingData);

            var messageId = Client.NextMessageId();

            var payloadWriter = new LLNetworkWriter();

            var length = fragmentSize + 8;
            payloadWriter.Write((ushort) length);
            payloadWriter.Write(messageId);

            payloadWriter.Write(fragmentedMessageId);
            payloadWriter.Write(i);
            payloadWriter.Write(fragmentAmount);

            var chunk = new byte[fragmentSize];
            Array.Copy(data, dataOffset, chunk, 0, fragmentSize);
            payloadWriter.Write(chunk);
            dataOffset += fragmentSize;

            var payload = payloadWriter.ToArray();
            writer.Write(payload);

            Client.StoreReliableMessage(messageId, ChannelId, payload);
        }
    }
}