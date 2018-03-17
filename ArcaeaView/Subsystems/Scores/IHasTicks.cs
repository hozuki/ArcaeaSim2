namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores {
    /// <summary>
    /// A note with a start tick and an end tick.
    /// </summary>
    public interface IHasTicks {

        /// <summary>
        /// Start tick, in milliseconds.
        /// </summary>
        int StartTick { get; }

        /// <summary>
        /// End tick, in milliseconds.
        /// </summary>
        int EndTick { get; }

    }
}
