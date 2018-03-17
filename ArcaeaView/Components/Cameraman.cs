using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using OpenMLTD.MilliSim.Foundation;

namespace Moe.Mottomo.ArcaeaSim.Components {
    /// <summary>
    /// The component that holds the main camera.
    /// </summary>
    public sealed class Cameraman : BaseGameComponent {

        public Cameraman([NotNull] BaseGame game, [CanBeNull] IBaseGameComponentContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Gets the camera held by this component.
        /// </summary>
        public Camera Camera { get; private set; }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            Camera.Update();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var camera = new Camera(Game.GraphicsDevice);

            Camera = camera;

            camera.Reset();
        }

    }
}
