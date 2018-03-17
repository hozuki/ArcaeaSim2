using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Moe.Mottomo.ArcaeaSim.Extensions {
    public static class Vector3Extensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XY(this Vector3 vector) {
            return new Vector2(vector.X, vector.Y);
        }

    }
}
