namespace SharpChannels.Core.Serialization
{
    internal static class Endianness
    {
        private static uint Swap(uint value)
        {
            value = (value >> 16) | (value << 16);
            return ((value & 0xFF00FF00U) >> 8) | ((value & 0x00FF00FFU) << 8);
        }

        public static void Swap(ref int value)
        {
            value = (int)Swap((uint)value);
        }

        public static void Swap(ref ushort value)
        {
            unchecked
            {
                value = (ushort)(((value & 0xFF00U) >> 8) | ((value & 0x00FFU) << 8));
            }
        }
    }
}