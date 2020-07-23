using System;

namespace StaticTools
{
    public static class StaticUtilities
    {
        public static bool IsPowerOfTwo(uint number) => number != 0 && (number & (number - 1)) == 0;

        public static int GetPowerOfTwo(uint number)
        {
            if (number <= 0)
            {
                throw new ArgumentException("Number must be positive!");
            }

            return (int)Math.Log(number & -number, 2);
        }
    }
}