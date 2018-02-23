namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <inheritdoc cref="NoteBase" />
    /// <inheritdoc cref="IHasTick"/>
    /// <summary>
    /// Represents a sky note, which is associated to an arc note.
    /// </summary>
    public sealed class SkyNote : NoteBase, IHasTick {

        public int Tick { get; set; }

        public override NoteType Type => NoteType.Sky;

    }
}
