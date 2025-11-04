using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal sealed class ReliableSequencedChannel : BaseChannel
{
    private readonly SortedDictionary<byte, byte[]> _pendingMessages = new();

    private byte _incomingSequenceNumber = 1;

    private byte _outgoingSequenceNumber = 1;

    public ReliableSequencedChannel(UNetClient client, byte channelId) : base(client, channelId)
    {
    }

    public override void Process(LLNetworkReader reader)
    {
        var length = reader.ReadUInt16();
        var messageId = reader.ReadUInt16();
        var orderedMessageId = reader.ReadByte();

        var payloadLength = length - 6;
        if (!Client.PacketAcks.ReceiveMessage(messageId))
        {
            SkipPayload(reader, payloadLength);
            return;
        }

        Client.SendAcks();

        // This is the next expected message, process it and any buffered messages in order
        if (orderedMessageId == _incomingSequenceNumber)
        {
            ReadPayload(reader, payloadLength);

            _incomingSequenceNumber++;

            // Process any buffered messages that can now be processed in order
            while (_pendingMessages.TryGetValue(_incomingSequenceNumber, out var bufferedPayload))
            {
                _pendingMessages.Remove(_incomingSequenceNumber);
                Client.EventListener.OnNetworkReceive(Client, new ArraySegment<byte>(bufferedPayload), ChannelId);
                _incomingSequenceNumber++;
            }
        }
        // This is a message from the future, buffer it for later processing
        else if (orderedMessageId > _incomingSequenceNumber || _incomingSequenceNumber == 255)
        {
            if (!_pendingMessages.ContainsKey(orderedMessageId))
            {
                var payload = reader.ReadBytes(payloadLength);
                _pendingMessages.Add(orderedMessageId, payload);
            }
        }
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var messageId = Client.NextMessageId();

        var payloadWriter = new LLNetworkWriter();

        var length = data.Length + 6;
        payloadWriter.Write((ushort) length);
        payloadWriter.Write(messageId);
        payloadWriter.Write(_outgoingSequenceNumber++);
        payloadWriter.Write(data);

        var payload = payloadWriter.ToArray();
        writer.Write(payload);

        Client.StoreReliableMessage(messageId, ChannelId, payload);
    }
}