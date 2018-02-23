using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;
using OpenMLTD.MilliSim.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Components {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class TrackDisplayFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.arcaea.track_display";

        public override string PluginName => "Track Display Factory";

        public override string PluginDescription => "Track Display Factory";

        public override string PluginAuthor => "hozuki";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new TrackDisplay(game, (IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
