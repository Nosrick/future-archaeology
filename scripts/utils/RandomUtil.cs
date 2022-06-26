using System;
using Godot;

namespace ATimeGoneBy.scripts.utils
{
    public static class RandomUtil
    {
        private static readonly Random RANDOM = new Random();

        private static readonly ulong START_TIME = OS.GetUnixTime();

        public static int Ticks => (int) (OS.GetUnixTime() - START_TIME);

        public static int PosNegCoinFlip()
        {
            return RANDOM.Next() % 2 == 0 ? -1 : 1;
        }
    }
}