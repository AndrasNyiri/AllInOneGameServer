using System;

namespace LightEngineCore.Common
{
    public static class MathHelper
    {
        public static Random rng = new Random();

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float NextFloat(float min, float max)
        {
            return Convert.ToSingle(rng.NextDouble() * (max - min) + min);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }
}
