public class BitMasking
{
    public static bool GetBit(ulong value, ulong mask)
    {
        return (value & mask) == mask;
    }
}
