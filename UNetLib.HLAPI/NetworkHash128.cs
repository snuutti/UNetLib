namespace UNetLib.HLAPI;

public struct NetworkHash128
{
    public byte I0;
    public byte I1;
    public byte I2;
    public byte I3;
    public byte I4;
    public byte I5;
    public byte I6;
    public byte I7;
    public byte I8;
    public byte I9;
    public byte I10;
    public byte I11;
    public byte I12;
    public byte I13;
    public byte I14;
    public byte I15;

    public void Reset()
    {
        I0 = 0;
        I1 = 0;
        I2 = 0;
        I3 = 0;
        I4 = 0;
        I5 = 0;
        I6 = 0;
        I7 = 0;
        I8 = 0;
        I9 = 0;
        I10 = 0;
        I11 = 0;
        I12 = 0;
        I13 = 0;
        I14 = 0;
        I15 = 0;
    }

    public bool IsValid()
    {
        return (I0 | I1 | I2 | I3 | I4 | I5 | I6 | I7 | I8 | I9 | I10 | I11 | I12 | I13 | I14 | I15) != 0;
    }

    public static int HexToNumber(char c)
    {
        if (c is >= '0' and <= '9')
        {
            return c - '0';
        }

        if (c is >= 'a' and <= 'f')
        {
            return c - 'a' + 10;
        }

        if (c is >= 'A' and <= 'F')
        {
            return c - 'A' + 10;
        }

        return 0;
    }

    public static NetworkHash128 Parse(string text)
    {
        NetworkHash128 hash;

        var length = text.Length;
        if (length < 32)
        {
            var tmp = "";
            for (var i = 0; i < 32 - length; i++)
            {
                tmp += "0";
            }

            text = tmp + text;
        }

        hash.I0 = (byte) (HexToNumber(text[0]) * 16 + HexToNumber(text[1]));
        hash.I1 = (byte) (HexToNumber(text[2]) * 16 + HexToNumber(text[3]));
        hash.I2 = (byte) (HexToNumber(text[4]) * 16 + HexToNumber(text[5]));
        hash.I3 = (byte) (HexToNumber(text[6]) * 16 + HexToNumber(text[7]));
        hash.I4 = (byte) (HexToNumber(text[8]) * 16 + HexToNumber(text[9]));
        hash.I5 = (byte) (HexToNumber(text[10]) * 16 + HexToNumber(text[11]));
        hash.I6 = (byte) (HexToNumber(text[12]) * 16 + HexToNumber(text[13]));
        hash.I7 = (byte) (HexToNumber(text[14]) * 16 + HexToNumber(text[15]));
        hash.I8 = (byte) (HexToNumber(text[16]) * 16 + HexToNumber(text[17]));
        hash.I9 = (byte) (HexToNumber(text[18]) * 16 + HexToNumber(text[19]));
        hash.I10 = (byte) (HexToNumber(text[20]) * 16 + HexToNumber(text[21]));
        hash.I11 = (byte) (HexToNumber(text[22]) * 16 + HexToNumber(text[23]));
        hash.I12 = (byte) (HexToNumber(text[24]) * 16 + HexToNumber(text[25]));
        hash.I13 = (byte) (HexToNumber(text[26]) * 16 + HexToNumber(text[27]));
        hash.I14 = (byte) (HexToNumber(text[28]) * 16 + HexToNumber(text[29]));
        hash.I15 = (byte) (HexToNumber(text[30]) * 16 + HexToNumber(text[31]));

        return hash;
    }

    public override string ToString()
    {
        return $"{I0:x2}{I1:x2}{I2:x2}{I3:x2}{I4:x2}{I5:x2}{I6:x2}{I7:x2}{I8:x2}{I9:x2}{I10:x2}{I11:x2}{I12:x2}{I13:x2}{I14:x2}{I15:x2}";
    }
}