using System.Diagnostics.CodeAnalysis;

namespace UNetLib.HLAPI;

public struct NetworkInstanceId
{
    public readonly uint Value;

    public NetworkInstanceId(uint value)
    {
        Value = value;
    }

    public bool IsEmpty()
    {
        return Value == 0;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is NetworkInstanceId id && this == id;
    }

    public override int GetHashCode()
    {
        return (int) Value;
    }

    public static bool operator ==(NetworkInstanceId c1, NetworkInstanceId c2)
    {
        return c1.Value == c2.Value;
    }

    public static bool operator !=(NetworkInstanceId c1, NetworkInstanceId c2)
    {
        return c1.Value != c2.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}