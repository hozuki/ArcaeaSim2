using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Configuration;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace Moe.Mottomo.ArcaeaSim.Components {
    /// <summary>
    /// Arcaea beatmap loader component.
    /// </summary>
    public sealed class BeatmapLoader : BaseGameComponent {

        public BeatmapLoader([NotNull] BaseGame game, [CanBeNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        public event EventHandler<BeatmapChangedEventArgs> BeatmapChanged;

        [CanBeNull]
        public Beatmap Beatmap { get; private set; }

        /// <summary>
        /// Loads an AFF beatmap from string.
        /// </summary>
        /// <param name="text">AFF beatmap content string.</param>
        public void LoadFrom([NotNull] string text) {
            Beatmap.TryParse(text, out var newBeatmap);

            var oldBeatmap = Beatmap;
            Beatmap = newBeatmap;

            BeatmapChanged?.Invoke(this, new BeatmapChangedEventArgs(oldBeatmap, newBeatmap));

            var game = Game.ToBaseGame();
            var debugOverlay = game.FindSingleElement<DebugOverlay>();

            if (debugOverlay != null) {
                if (newBeatmap != null) {
                    debugOverlay.AddLine($"Loaded beatmap from string.");
                } else {
                    debugOverlay.AddLine($"Failed to load beatmap from string.");
                }
            }
        }

        /// <summary>
        /// Loads an AFF beatmap from file.
        /// </summary>
        /// <param name="filePath">Path to the file that contains AFF beatmap data.</param>
        public void Load([NotNull] string filePath) {
            Beatmap newBeatmap;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream, Encoding.UTF8)) {
                    Beatmap.TryParse(reader, out newBeatmap);
                }
            }

            var oldBeatmap = Beatmap;
            Beatmap = newBeatmap;

            BeatmapChanged?.Invoke(this, new BeatmapChangedEventArgs(oldBeatmap, newBeatmap));

            var game = Game.ToBaseGame();
            var debugOverlay = game.FindSingleElement<DebugOverlay>();

            if (debugOverlay != null) {
                if (newBeatmap != null) {
                    debugOverlay.AddLine($"Loaded beatmap from '{filePath}'.");
                } else {
                    debugOverlay.AddLine($"Failed to load beatmap from '{filePath}'.");
                }
            }
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var config = ConfigurationStore.Get<BeatmapLoaderConfig>();

            Load(config.Data.FilePath);
        }

    }
}

