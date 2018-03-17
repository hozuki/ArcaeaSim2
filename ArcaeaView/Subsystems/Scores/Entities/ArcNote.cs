using JetBrains.Annotations;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <inheritdoc cref="NoteBase" />
    /// <inheritdoc cref="IHasTicks"/>
    /// <summary>
    /// Represents an arc note.
    /// </summary>
    public sealed class ArcNote : NoteBase, IHasTicks {

        public int StartTick { get; set; }

        public int EndTick { get; set; }

        /// <summary>
        /// Start X position. The playable region should be [-0.5, 1.5].
        /// </summary>
        public float StartX { get; set; }

        /// <summary>
        /// End X position. The playable region should be [-0.5, 1.5].
        /// </summary>
        public float EndX { get; set; }

        /// <summary>
        /// Easing method.
        /// </summary>
        public ArcEasing Easing { get; set; }

        /// <summary>
        /// Start Y position. The playable region should be [0, 1].
        /// </summary>
        public float StartY { get; set; }

        /// <summary>
        /// End Y position. The playable region should be [0, 1].
        /// </summary>
        public float EndY { get; set; }

        /// <summary>
        /// Color of the arc. Only matters if <see cref="IsPlayable"/> is <see langword="true"/>.
        /// </summary>
        public ArcColor Color { get; set; }

        /// <summary>
        /// Unknown field. The most common value is <code>"none"</code>.
        /// </summary>
        public string Unknown1 { get; set; }

        /// <summary>
        /// Whether this note a trace arc (i.e. not playble, translucent).
        /// </summary>
        public bool IsTraceArc { get; set; }

        /// <summary>
        /// Whether this note a playable arc (i.e. must be tracked with your fingers).
        /// </summary>
        public bool IsPlayable => !IsTraceArc;

        /// <summary>
        /// The sky notes associated with this note.
        /// The array has a value (i.e. is not null) only when <see cref="IsTraceArc"/> is <see langword="true"/>.
        /// </summary>
        [CanBeNull]
        public SkyNote[] SkyNotes { get; set; }

        public override NoteType Type => NoteType.Arc;

    }
}
