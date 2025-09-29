using UNetLib.LLAPI;

namespace UNetLib.Channel;

internal sealed class UnreliableChannel : BaseChannel
{
    public UnreliableChannel(UNetClient client, byte channelId) : base(client, channelId)
    {
    }

    public override void Process(LLNetworkReader reader)
    {
        var length = reader.ReadUInt16();
        var payloadLength = length - 3;
        ReadPayload(reader, payloadLength);
    }

    public override void Prepare(LLNetworkWriter writer, byte[] data)
    {
        var length = data.Length + 3;
        writer.Write((ushort) length);
        writer.Write(data);
    }
}