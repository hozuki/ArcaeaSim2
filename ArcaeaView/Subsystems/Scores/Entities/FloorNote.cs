namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <inheritdoc cref="NoteBase" />
    /// <inheritdoc cref="IHasTick"/>
    /// <summary>
    /// Represents a floor note.
    /// </summary>
    public sealed class FloorNote : NoteBase, IHasTick {

        public int Tick { get; set; }

        /// <summary>
        /// The track that this note is on. 
        /// </summary>
        public Track Track { get; set; }

        public override NoteType Type => NoteType.Floor;

    }
}
