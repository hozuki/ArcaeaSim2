namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores {
    /// <summary>
    /// A note with only one tick.
    /// </summary>
    public interface IHasTick {

        /// <summary>
        /// Tick of this note, in milliseconds.
        /// </summary>
        int Tick { get; }

    }
}
