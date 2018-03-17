using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;

namespace Moe.Mottomo.ArcaeaSim.Components {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class CameramanFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.arcaea.cameraman";

        public override string PluginName => "Cameraman Factory";

        public override string PluginDescription => "Cameraman Factory";

        public override string PluginAuthor => "hozuki";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new Cameraman(game, parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
