using System;
using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Components {
    public sealed class BeatmapChangedEventArgs : EventArgs {

        internal BeatmapChangedEventArgs([CanBeNull] Beatmap oldBeatmap, [CanBeNull] Beatmap newBeatmap) {
            OldBeatmap = oldBeatmap;
            NewBeatmap = newBeatmap;
        }

        [CanBeNull]
        public Beatmap OldBeatmap { get; }

        [CanBeNull]
        public Beatmap NewBeatmap { get; }

    }
}
