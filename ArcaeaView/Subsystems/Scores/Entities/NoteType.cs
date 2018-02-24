namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <summary>
    /// Note types.
    /// </summary>
    public enum NoteType {

        /// <summary>
        /// A floor note. It appears at the bottom of the game area. It should be tapped.
        /// </summary>
        Floor = 0,
        /// <summary>
        /// A long note. It appears at the bottom of the game area. It should be pressed on start tick and released on end tick.
        /// </summary>
        Long = 1,
        /// <summary>
        /// An arc note. The complicated and special note type of Arcaea.
        /// </summary>
        Arc = 2,
        /// <summary>
        /// A timing note. It changes the tempo (BPM) information.
        /// </summary>
        Timing = 3,
        /// <summary>
        /// A sky note. It does not appear in the beatmap. This value is only for convenience of development.
        /// </summary>
        Sky = 4,
        Max = 4

    }
}
