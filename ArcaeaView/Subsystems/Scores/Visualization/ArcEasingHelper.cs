using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Moe.Mottomo.ArcaeaSim.Core;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <summary>
    /// Arc note easing helper functions.
    /// </summary>
    public static class ArcEasingHelper {

        /// <summary>
        /// Calculates the eased value of a pair of coordinates.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <param name="t">A value increasing from 0 to 1 while moving from <see cref="start"/> to <see cref="end"/>.</param>
        /// <param name="easing">Easing method.</param>
        /// <returns>Eased coordinate.</returns>
        public static Vector3 Ease(Vector3 start, Vector3 end, float t, ArcEasing easing) {
            t = MathHelper.Clamp(t, 0, 1);

            switch (easing) {
                case ArcEasing.S:
                case ArcEasing.SoSo:
                case ArcEasing.SiSi:
                    return EaseLinear(start, end, t);
                case ArcEasing.CubicBezier:
                    return EaseCubicBezier(start, end, t);
                case ArcEasing.Si:
                case ArcEasing.So:
                case ArcEasing.SiSo:
                case ArcEasing.SoSi:
                    return EaseSinus(start, end, t, easing);
                default:
                    throw new ArgumentOutOfRangeException(nameof(easing), easing, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 EaseLinear(Vector3 p1, Vector3 p2, float t) {
            return Vector3.Lerp(p1, p2, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 EaseCubicBezier(Vector3 p1, Vector3 p2, float t) {
            var mt = 1 - t;
            var mt2 = mt * mt;
            var mt3 = mt2 * mt;
            var t2 = t * t;
            var t3 = t2 * t;

            var d = p2.Y - p1.Y;
            var cp1 = new Vector3(p1.X, p1.Y + d / 4, p1.Z);
            var cp2 = new Vector3(p2.X, p2.Y - d / 4, p2.Z);

            var result = mt3 * p1 + 3 * mt2 * t * cp1 + 3 * mt * t2 * cp2 + t3 * p2;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 EaseSinus(Vector3 p1, Vector3 p2, float t, ArcEasing easing) {
            var y = MathHelper.Lerp(p1.Y, p2.Y, t);

            float sx, sz;

            switch (easing) {
                case ArcEasing.Si:
                case ArcEasing.SiSi:
                case ArcEasing.SiSo:
                    sx = MathF.Sin(t * MathHelper.PiOver2);
                    break;
                case ArcEasing.So:
                case ArcEasing.SoSi:
                case ArcEasing.SoSo:
                    sx = 1 - MathF.Sin((1 + t) * MathHelper.PiOver2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(easing), easing, null);
            }

            switch (easing) {
                case ArcEasing.Si:
                case ArcEasing.So:
                case ArcEasing.SoSi:
                case ArcEasing.SiSi:
                    sz = MathF.Sin(t * MathHelper.PiOver2);
                    break;
                case ArcEasing.SiSo:
                case ArcEasing.SoSo:
                    sz = 1 - MathF.Sin((1 + t) * MathHelper.PiOver2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(easing), easing, null);
            }

            var dx = p2.X - p1.X;
            var x = p1.X + dx * sx;

            var dz = p2.Z - p1.Z;
            var z = p1.Z + dz * sz;

            return new Vector3(x, y, z);
        }

    }
}
