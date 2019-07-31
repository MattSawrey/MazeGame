using System;
using System.Collections.Generic;

namespace Maze_Game_Common.Extension_Methods
{
    public static class CollectionExtenions
    {
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T SelectRandom<T>(this T[] list, Random rng)
        {
            int randIndex = rng.Next(0, list.Length);
            return list[randIndex];
        }
    }
}
