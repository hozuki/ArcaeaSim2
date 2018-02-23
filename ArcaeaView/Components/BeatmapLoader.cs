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

        /// <summary>
        /// Loads an AFF beatmap from string.
        /// </summary>
        /// <param name="text">AFF beatmap content string.</param>
        public void Load([NotNull] string text) {
            Beatmap.TryParse(text, out var beatmap);

            Beatmap = beatmap;

            var game = Game.ToBaseGame();
            var debugOverlay = game.FindSingleElement<DebugOverlay>();

            if (debugOverlay != null) {
                if (beatmap != null) {
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
        public void LoadFrom([NotNull] string filePath) {
            Beatmap beatmap;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream, Encoding.UTF8)) {
                    Beatmap.TryParse(reader, out beatmap);
                }
            }

            Beatmap = beatmap;

            var game = Game.ToBaseGame();
            var debugOverlay = game.FindSingleElement<DebugOverlay>();

            if (debugOverlay != null) {
                if (beatmap != null) {
                    debugOverlay.AddLine($"Loaded beatmap from '{filePath}'.");
                } else {
                    debugOverlay.AddLine($"Failed to load beatmap from '{filePath}'.");
                }
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var config = ConfigurationStore.Get<BeatmapLoaderConfig>();

            LoadFrom(config.Data.FilePath);
        }

        [CanBeNull]
        public Beatmap Beatmap { get; private set; }

    }
}

