using System;
using System.Collections.Generic;
using System.Text;

namespace PTG.src.utility
{
    public class RandomHelper
    {
        private static readonly Random random = new Random();

        public static float RandFloat()
        {
            return (float)random.NextDouble();
        }

        public static int RandInt(int min, int max)
        {
            if (min > max)
                throw new ArgumentException("Min value must be smaller than max value!");

            if (min < 0)
                return random.Next(0, max + 1 - min) + min;
            return random.Next(min, max + 1);
        }

        public static T Choice<T>(List<T> items)
        {
            return items[RandInt(0, items.Count - 1)];
        }
    }
}
