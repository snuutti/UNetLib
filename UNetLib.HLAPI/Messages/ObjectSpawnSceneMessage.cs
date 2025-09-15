using System.Numerics;

namespace UNetLib.HLAPI.Messages;

public class ObjectSpawnSceneMessage : IMessageBase
{
    public NetworkInstanceId NetId { get; set; }
    public NetworkSceneId SceneId { get; set; }
    public Vector3 Position { get; set; }
    public byte[] Payload { get; set; } = [];

    public void Deserialize(NetworkReader reader)
    {
        NetId = reader.ReadNetworkId();
        SceneId = reader.ReadSceneId();
        Position = reader.ReadVector3();
        Payload = reader.ReadBytesAndSize()!;
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(NetId);
        writer.Write(SceneId);
        writer.Write(Position);
        writer.WriteBytesFull(Payload);
    }

    public override string ToString()
    {
        return $"ObjectSpawnSceneMessage(NetId={NetId}, SceneId={SceneId}, Position={Position}, PayloadSize={(Payload.Length)})";
    }
}