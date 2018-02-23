using Microsoft.Xna.Framework;

namespace Moe.Mottomo.ArcaeaSim.Extensions {
    public static class ColorExtensions {

        /// <summary>
        /// Return a new color whose brighness is changed from a base color.
        /// </summary>
        /// <param name="color">Base color.</param>
        /// <param name="amount">A value between [-255, 255] </param>
        /// <returns></returns>
        public static Color ChangeBrightness(this Color color, int amount) {
            var a = color.A;
            var r = MathHelper.Clamp(color.R + amount, 0, 255);
            var g = MathHelper.Clamp(color.G + amount, 0, 255);
            var b = MathHelper.Clamp(color.B + amount, 0, 255);

            return new Color(r, g, b, a);
        }

    }
}
