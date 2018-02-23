using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <summary>
    /// A drawable note protocol.
    /// </summary>
    public interface IDrawableNote {

        /// <summary>
        /// Draws this note.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/> to use.</param>
        /// <param name="beatmapTicks">Current beatmap time, in milliseconds. Please note that beatmap time does not equal to audio time when a beatmap's audio offset is non-zero.</param>
        /// <param name="currentY">Current Y position of the ongoing beatmap.</param>
        void Draw([NotNull] Effect effect, int beatmapTicks, float currentY);

        /// <summary>
        /// Determines whether this note is visible or not.
        /// </summary>
        /// <param name="beatmapTicks">Current beatmap time, in milliseconds. Please note that beatmap time does not equal to audio time when a beatmap's audio offset is non-zero.</param>
        /// <param name="currentY">Current Y position of the ongoing beatmap.</param>
        /// <returns><see langword="true"/> if the note is visible, otherwise <see langword="false"/>.</returns>
        bool IsVisible(int beatmapTicks, float currentY);

    }
}
