using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;

namespace Moe.Mottomo.ArcaeaSim.Components {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class BeatmapLoaderFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.arcaea.beatmap_loader";

        public override string PluginName => "Beatmap Loader Factory";

        public override string PluginDescription => "Arcaea Beatmap Loader Factory";

        public override string PluginAuthor => "hozuki";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new BeatmapLoader(game, parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
