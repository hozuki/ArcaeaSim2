using OpenMLTD.MilliSim.Configuration.Entities;

namespace Moe.Mottomo.ArcaeaSim.Configuration {
    public sealed class BeatmapLoaderConfig : ConfigBase {

        public ScoreLoaderConfigData Data { get; set; }

        public sealed class ScoreLoaderConfigData {

            public string Title { get; set; }

            public string FilePath { get; set; }

        }

    }
}
