namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores {
    /// <summary>
    /// A preview note with its Y position precalculated.
    /// </summary>
    public interface IPreviewNote {

        /// <summary>
        /// Precalculated Y position.
        /// </summary>
        float PreviewY { get; }

    }
}
