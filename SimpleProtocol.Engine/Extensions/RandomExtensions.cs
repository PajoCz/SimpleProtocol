using System;

namespace SimpleProtocol.Engine.Extensions
{
    public static class RandomExtensions
    {
        public static long NextLong(this Random rnd)
        {
            byte[] buffer = new byte[8];
            rnd.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
