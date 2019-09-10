using System;

public class BitHelper
{
    public static int getIntFromPackedByte(byte input, int start, int end)
    {
        int size = end - start;
        int shiftAmount = 8 - end;
        int mask = (1 << size) - 1;
        int result = (input >> shiftAmount) & mask;
        return result;
    }

    public static int getInt16FromBytes(byte[] bytes, int offset)
    {
        return BitConverter.ToInt16(new byte[] { bytes[offset], bytes[offset + 1] }, 0);
    }
}
