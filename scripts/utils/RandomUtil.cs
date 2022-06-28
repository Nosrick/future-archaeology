using System;
using Godot;

namespace ATimeGoneBy.scripts.utils
{
    public static class RandomUtil
    {
        private static readonly Random RANDOM = new Random();

        private static readonly ulong START_TIME = OS.GetTicksMsec();

        public static int RunningTicks => (int) (OS.GetTicksMsec() - START_TIME) / 1000;
        public static int SystemTicks => (int) (OS.GetTicksMsec() / 1000);

        public static int PosNegCoinFlip()
        {
            return RANDOM.Next() % 2 == 0 ? -1 : 1;
        }
    }
}