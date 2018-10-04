using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightEngineSerializeable.Utils
{
    public static class Extensions
    {
        private const double DEGREES_TO_RADIANS = (Math.PI / 180);
        private const float FLOAT_TO_SHORT_MULTIPLIER = 100f;

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public static short ToShort(this float f)
        {
            return Convert.ToInt16(f * FLOAT_TO_SHORT_MULTIPLIER);
        }

        public static float ToFloat(this short s)
        {
            return s / FLOAT_TO_SHORT_MULTIPLIER;
        }

        public static float ToRadians(this float f)
        {
            return (float)(f * DEGREES_TO_RADIANS);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }
}
