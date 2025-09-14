namespace UNetLib.HLAPI.Messages;

public class CrcMessage : IMessageBase
{
    public List<CrcMessageEntry> Scripts { get; set; } = [];

    public void Deserialize(NetworkReader reader)
    {
        var numScripts = reader.ReadUInt16();
        for (var i = 0; i < numScripts; i++)
        {
            var script = new CrcMessageEntry
            {
                Name = reader.ReadString(),
                Channel = reader.ReadByte()
            };

            Scripts.Add(script);
        }
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write((ushort) Scripts.Count);
        foreach (var script in Scripts)
        {
            writer.Write(script.Name);
            writer.Write(script.Channel);
        }
    }

    public override string ToString()
    {
        return $"CrcMessage(Scripts={string.Join(",", Scripts.Select(s => $"{s.Name}:{s.Channel}"))})";
    }

    public struct CrcMessageEntry
    {
        public string Name;
        public byte Channel;
    }
}