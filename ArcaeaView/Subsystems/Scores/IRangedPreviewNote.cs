namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores {
    /// <summary>
    /// A preview note with its Y positions at start and the end are precalculated.
    /// </summary>
    public interface IRangedPreviewNote {

        /// <summary>
        /// Precalculated starting Y position.
        /// </summary>
        float PreviewStartY { get; }

        /// <summary>
        /// Precalculated ending Y position.
        /// </summary>
        float PreviewEndY { get; }

    }
}
