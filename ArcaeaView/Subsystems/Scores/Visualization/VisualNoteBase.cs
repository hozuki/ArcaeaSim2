using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;
using OpenMLTD.MilliSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="DisposableBase" />
    /// <inheritdoc cref="IDrawableNote"/>
    /// <summary>
    /// Visual note base.
    /// </summary>
    public abstract class VisualNoteBase : DisposableBase, IDrawableNote {

        protected VisualNoteBase([NotNull] VisualBeatmap beatmap, [NotNull] NoteBase baseNote) {
            Beatmap = beatmap;
            BaseNote = baseNote;
            Type = baseNote.Type;
        }

        /// <summary>
        /// Gets the <see cref="VisualBeatmap"/> that this note belongs to.
        /// </summary>
        public VisualBeatmap Beatmap { get; }

        /// <summary>
        /// Gets the underlying <see cref="NoteBase"/>.
        /// </summary>
        public NoteBase BaseNote { get; }

        /// <summary>
        /// Gets the type of this note.
        /// </summary>
        public NoteType Type { get; }

        public abstract void Draw(Effect effect, int beatmapTicks, float currentY);

        public abstract bool IsVisible(int beatmapTicks, float currentY);

    }
}
