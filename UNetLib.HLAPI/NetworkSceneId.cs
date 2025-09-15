using System.Diagnostics.CodeAnalysis;

namespace UNetLib.HLAPI;

public struct NetworkSceneId
{
    public readonly uint Value;

    public NetworkSceneId(uint value)
    {
        Value = value;
    }
    
    public bool IsEmpty()
    {
        return Value == 0;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is NetworkSceneId id && this == id;
    }

    public override int GetHashCode()
    {
        return (int) Value;
    }

    public static bool operator ==(NetworkSceneId c1, NetworkSceneId c2)
    {
        return c1.Value == c2.Value;
    }

    public static bool operator !=(NetworkSceneId c1, NetworkSceneId c2)
    {
        return c1.Value != c2.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}