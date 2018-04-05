using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Bvs {
    public sealed class ArcCommunication : DisposableBase {

        internal ArcCommunication([NotNull] ArcaeaSimApplication game) {
            Game = game;
            Server = new ArcServer(this);
            Client = new ArcClient(this);
        }

        [NotNull]
        public ArcaeaSimApplication Game { get; }

        [CanBeNull]
        public Uri EditorServerUri { get; internal set; }

        [NotNull]
        public ArcServer Server { get; }

        [NotNull]
        public ArcClient Client { get; }

        protected override void Dispose(bool disposing) {
            Server.Dispose();
            Client.Dispose();
        }

    }
}
