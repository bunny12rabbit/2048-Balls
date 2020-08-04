using System;
using UnityEngine;

namespace StaticTools
{
    public static class StaticUtilities
    {
        /// <summary>
        /// Checks whether given number is PoT (Power of Two)
        /// </summary>
        /// <param name="number">Number to check</param>
        /// <returns>True if number is PoT, Else otherwise</returns>
        public static bool IsPowerOfTwo(uint number) => number != 0 && (number & (number - 1)) == 0;

        
        /// <summary>
        /// Gives rounded to int number, which represents PoT (Power of Two) of the given number 
        /// </summary>
        /// <param name="number">Number to find PoT of</param>
        /// <returns>PoT</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetPowerOfTwo(uint number)
        {
            if (number <= 0)
            {
                throw new ArgumentException("Number must be positive!");
            }

            return (int)Math.Log(number & -number, 2);
        }
        
        
        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.r;
            float green = color.g;
            float blue = color.b;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return new Color(red, green, blue, color.a);
        }
    }
}