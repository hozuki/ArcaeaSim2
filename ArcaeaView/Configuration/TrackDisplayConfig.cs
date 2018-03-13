using OpenMLTD.MilliSim.Configuration.Entities;

namespace Moe.Mottomo.ArcaeaSim.Configuration {
    public sealed class TrackDisplayConfig : ConfigBase {

        public TrackDisplayConfigData Data { get; set; }

        public sealed class TrackDisplayConfigData {

            public string PanelTexture { get; set; }

            public string TrackLaneDividerTexture { get; set; }

            public string SkyInputTexture { get; set; }

            public string NoteTexture { get; set; }

            public string NoteHoldTexture { get; set; }

            public string NoteHoldHighlightedTexture { get; set; }

            public string NoteSkyTexture { get; set; }

        }

    }
}
