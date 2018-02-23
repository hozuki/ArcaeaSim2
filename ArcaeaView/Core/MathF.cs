using System;
using System.Runtime.CompilerServices;

namespace Moe.Mottomo.ArcaeaSim.Core {
    /// <summary>
    /// Floating point number helper functions.
    /// </summary>
    public static class MathF {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float radian) {
            return (float)Math.Cos(radian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float radian) {
            return (float)Math.Sin(radian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float d) {
            return (float)Math.Atan(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float radian) {
            return (float)Math.Tan(radian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampLower(float value, float lowerBound) {
            return value < lowerBound ? lowerBound : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampUpper(float value, float upperBound) {
            return value > upperBound ? upperBound : value;
        }

    }
}
