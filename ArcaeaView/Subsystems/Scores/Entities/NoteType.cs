namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <summary>
    /// Note types.
    /// </summary>
    public enum NoteType {

        /// <summary>
        /// A floor note. It appears at the bottom of the game area. It should be tapped.
        /// </summary>
        Floor,
        /// <summary>
        /// A long note. It appears at the bottom of the game area. It should be pressed on start tick and released on end tick.
        /// </summary>
        Long,
        /// <summary>
        /// An arc note. The complicated and special note type of Arcaea.
        /// </summary>
        Arc,
        /// <summary>
        /// A timing note. It changes the tempo (BPM) information.
        /// </summary>
        Timing

    }
}
