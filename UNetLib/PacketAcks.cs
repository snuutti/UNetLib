namespace UNetLib;

internal class PacketAcks
{
    private readonly bool _acksLong;

    private readonly HashSet<ushort> _missingMessages = [];

    private ushort _lastReceivedMessageId;

    public PacketAcks(bool acksLong)
    {
        _acksLong = acksLong;
    }

    public bool ReceiveMessage(ushort messageId)
    {
        if (_lastReceivedMessageId == messageId)
        {
            return false;
        }

        _missingMessages.Remove(messageId);

        if (messageId > _lastReceivedMessageId)
        {
            for (var i = _lastReceivedMessageId + 1; i < messageId; i++)
            {
                _missingMessages.Add((ushort) i);
            }

            _lastReceivedMessageId = messageId;
        }

        return true;
    }

    public (List<ushort> messageIdsToResend, List<ushort> receivedIds) ReadIncomingAcks(ushort ackMessageId, uint[] acks)
    {
        var messageIdsToResend = new List<ushort>();
        var receivedIds = new List<ushort>();

        if (!_acksLong)
        {
            var mask = acks[0];
            for (var i = 0; i < 32; i++)
            {
                var messageId = (ushort) (ackMessageId - i);
                if ((mask & (1u << i)) == 0)
                {
                    messageIdsToResend.Add(messageId);
                }
                else
                {
                    receivedIds.Add(messageId);
                }
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        return (messageIdsToResend, receivedIds);
    }

    public (ushort lastReceivedMessageId, uint[] acks) GetAcks()
    {
        if (!_acksLong)
        {
            var mask = 0xFFFFFFFF;
            foreach (var missingId in _missingMessages)
            {
                var bitPosition = _lastReceivedMessageId - missingId;
                if (bitPosition is >= 0 and < 32)
                {
                    mask &= ~(1u << bitPosition);
                }
            }

            return (_lastReceivedMessageId, [mask]);
        }

        throw new NotImplementedException();
    }
}