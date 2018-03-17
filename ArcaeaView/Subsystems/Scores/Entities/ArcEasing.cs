namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <summary>
    /// Arc easing method.
    /// </summary>
    public enum ArcEasing {

        /// <summary>
        /// Linear.
        /// </summary>
        S = 0,

        /// <summary>
        /// Cubic bezier, with control points at 1/4 and 3/4.
        /// </summary>
        CubicBezier = 1,

        /// <summary>
        /// Ease in as a quarter circle, then linear.
        /// </summary>
        Si = 2,
        /// <summary>
        /// Linear, then ease out as a quarter circle.
        /// </summary>
        So = 3,

        /// <summary>
        /// Ease in as a quarter circle.
        /// </summary>
        SiSo = 4,
        /// <summary>
        /// Ease out as a quarter circle.
        /// </summary>
        SoSi = 5,

        /// <summary>
        /// Behavior unknown.
        /// </summary>
        SiSi = 6,
        /// <summary>
        /// Behavior unknown.
        /// </summary>
        SoSo = 7

    }
}
