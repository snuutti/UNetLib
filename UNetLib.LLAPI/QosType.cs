namespace UNetLib.LLAPI;

public enum QosType
{
    /// <summary>
    /// There is no guarantee of delivery or ordering.
    /// </summary>
    Unreliable,
    /// <summary>
    /// There is no guarantee of delivery or ordering, but allowing fragmented messages with up to 32 fragments per message.
    /// </summary>
    UnreliableFragmented,
    /// <summary>
    /// There is no guarantee of delivery and all unordered messages will be dropped. Example: VoIP.
    /// </summary>
    UnreliableSequenced,
    /// <summary>
    /// Each message is guaranteed to be delivered but not guaranteed to be in order.
    /// </summary>
    Reliable,
    /// <summary>
    /// Each message is guaranteed to be delivered, also allowing fragmented messages with up to 32 fragments per message.
    /// </summary>
    ReliableFragmented,
    /// <summary>
    /// Each message is guaranteed to be delivered and in order.
    /// </summary>
    ReliableSequenced,
    /// <summary>
    /// An unreliable message. Only the last message in the send buffer is sent. Only the most recent message in the receive buffer will be delivered.
    /// </summary>
    StateUpdate,
    /// <summary>
    /// A reliable message. Note: Only the last message in the send buffer is sent. Only the most recent message in the receive buffer will be delivered.
    /// </summary>
    ReliableStateUpdate,
    /// <summary>
    /// A reliable message that will be re-sent with a high frequency until it is acknowledged.
    /// </summary>
    AllCostDelivery
}