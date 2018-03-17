using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <summary>
    /// Stage metrics that controls the general track layout.
    /// </summary>
    /// <remarks>
    /// This type is class rather than structure is because the same instance is frequently passed between objects.
    /// So one reference on the heap is enough, and can avoid extra stack allocation overhead.
    /// </remarks>
    [NotNull]
    public sealed class StageMetrics {

        /// <summary>
        /// Creates a new <see cref="StageMetrics"/> instance.
        /// </summary>
        public StageMetrics() {
            TrackLength = 100;
            TrackInnerWidth = 20;

            LaneDividerWidth = 0.2f;

            FloorNoteWidth = TrackInnerWidth / 64 * 15;
            FloorNoteHeight = TrackInnerWidth / 8;

            SkyNoteWidth = TrackInnerWidth / 4;
            SkyNoteHeight = SkyNoteWidth / 4;
            SkyNoteTallness = SkyNoteHeight / 2;

            FinishLineY = 10;
            FinishLineHeight = 2f;

            SkyInputZ = TrackInnerWidth / 3.236f;

            SkyInputWidth = TrackInnerWidth * 1.25f;
            SkyInputTallness = 0.8f;

            PlayableArcWidth = 2f;
            PlayableArcTallness = 2f;

            TraceArcWidth = 0.25f;
            TraceArcTallness = 0.25f;

            PastTickThreshold = 0;
            FutureTickThreshold = 4000;

            Speed = 5;
        }

        /// <summary>
        /// Length of the track.
        /// </summary>
        public float TrackLength { get; set; }

        /// <summary>
        /// Full width of the track.
        /// </summary>
        public float TrackFullWidth {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TrackInnerWidth / (1 - (72f / 1024)); // this ratio can be measured from track texture
        }

        /// <summary>
        /// Half of the full width of the track.
        /// </summary>
        public float HalfTrackFullWidth {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TrackFullWidth / 2;
        }

        /// <summary>
        /// Inner width of the track (excluding the borders).
        /// </summary>
        public float TrackInnerWidth { get; set; }

        /// <summary>
        /// Half of the inner width of the track.
        /// </summary>
        public float HalfTrackInnerWidth {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TrackInnerWidth / 2;
        }

        /// <summary>
        /// Width of a lane divider.
        /// </summary>
        public float LaneDividerWidth { get; set; }

        /// <summary>
        /// Width of a floor note.
        /// </summary>
        public float FloorNoteWidth { get; set; }

        /// <summary>
        /// Height of a floor note.
        /// </summary>
        public float FloorNoteHeight { get; set; }

        /// <summary>
        /// Width of a sky note.
        /// </summary>
        public float SkyNoteWidth { get; set; }

        /// <summary>
        /// Height of a sky note.
        /// </summary>
        public float SkyNoteHeight { get; set; }

        /// <summary>
        /// Tallness of a sky note.
        /// </summary>
        public float SkyNoteTallness { get; set; }

        /// <summary>
        /// Y position of the finish line.
        /// </summary>
        public float FinishLineY { get; set; }

        /// <summary>
        /// Height of the finish line.
        /// </summary>
        public float FinishLineHeight { get; set; }

        /// <summary>
        /// Z position of the sky input.
        /// </summary>
        public float SkyInputZ { get; set; }

        /// <summary>
        /// Width of the sky input.
        /// </summary>
        public float SkyInputWidth { get; set; }

        /// <summary>
        /// Tallness of the sky input.
        /// </summary>
        public float SkyInputTallness { get; set; }

        /// <summary>
        /// Playable arc section width.
        /// </summary>
        public float PlayableArcWidth { get; set; }

        /// <summary>
        /// Playable arc section tallness.
        /// </summary>
        public float PlayableArcTallness { get; set; }

        /// <summary>
        /// Trace arc section width.
        /// </summary>
        public float TraceArcWidth { get; set; }

        /// <summary>
        /// Trace arc tallness.
        /// </summary>
        public float TraceArcTallness { get; set; }

        /// <summary>
        /// Past tick threshold, in milliseconds. Notes whose tick or ending tick is before <code>beatmapTick + PastTickThreshold</code> will not be drawn.
        /// </summary>
        public int PastTickThreshold { get; set; }

        /// <summary>
        /// Future tick threshold, in milliseconds. Notes whose tick or starting tick is after <code>beatmapTick + FutureTickThreshold</code> will not be drawn.
        /// </summary>
        /// <remarks>
        /// Calculating this value is challenging.
        /// A value too large will significantly decrease the rendering efficiency.
        /// A value too small will cause notes after a sudden tempo change disappear.
        /// This value is related to <see cref="Speed"/>. When <see cref="Speed"/> increases, the required value of <see cref="FutureTickThreshold"/> increases.
        /// </remarks>
        public int FutureTickThreshold { get; set; }

        /// <summary>
        /// Note falling speed. Corresponding to the setting in Arcaea.
        /// </summary>
        public float Speed { get; set; }

    }
}
