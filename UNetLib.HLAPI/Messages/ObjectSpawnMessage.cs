using System.Numerics;

namespace UNetLib.HLAPI.Messages;

public class ObjectSpawnMessage : IMessageBase
{
    public NetworkInstanceId NetId { get; set; }
    public NetworkHash128 AssetId { get; set; }
    public Vector3 Position { get; set; }
    public byte[]? Payload { get; set; }
    public Quaternion? Rotation { get; set; }

    public void Deserialize(NetworkReader reader)
    {
        NetId = reader.ReadNetworkId();
        AssetId = reader.ReadNetworkHash128();
        Position = reader.ReadVector3();
        Payload = reader.ReadBytesAndSize();

        if (reader.Length - reader.Position < 16)
        {
            return;
        }

        Rotation = reader.ReadQuaternion();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(NetId);
        writer.Write(AssetId);
        writer.Write(Position);
        writer.WriteBytesFull(Payload);

        if (Rotation != null)
        {
            writer.Write(Rotation.Value);
        }
    }

    public override string ToString()
    {
        return $"ObjectSpawnMessage(NetId={NetId}, AssetId={AssetId}, Position={Position}, PayloadLength={Payload?.Length ?? 0}, Rotation={Rotation})";
    }
}