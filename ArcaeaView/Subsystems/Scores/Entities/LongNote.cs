namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <inheritdoc cref="NoteBase" />
    /// <inheritdoc cref="IHasTicks"/>
    /// <summary>
    /// Represents a long note.
    /// </summary>
    public sealed class LongNote : NoteBase, IHasTicks {

        public int StartTick { get; set; }

        public int EndTick { get; set; }

        /// <summary>
        /// The track that this note is on.
        /// </summary>
        public Track Track { get; set; }

        public override NoteType Type => NoteType.Long;

    }
}
