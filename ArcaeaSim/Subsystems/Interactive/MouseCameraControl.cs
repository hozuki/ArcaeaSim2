using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moe.Mottomo.ArcaeaSim.Components;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Interactive {
    public sealed class MouseCameraControl : GameComponent {

        public MouseCameraControl(Game game)
            : base(game) {
        }

        public override void Initialize() {
            base.Initialize();

            _camera = Game.ToBaseGame().FindSingleElement<Cameraman>()?.Camera;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (_camera == null) {
                return;
            }

            var currentMouseState = Mouse.GetState();

            var lastMouseState = _lastMoustState;
            _lastMoustState = currentMouseState;

            // When the user is holding the right key, move our camera.
            if (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed) {
                float xDifference = currentMouseState.X - lastMouseState.X;
                float yDifference = currentMouseState.Y - lastMouseState.Y;

                _camera.Pitch(RotationSpeed * yDifference);
                _camera.Yaw(-RotationSpeed * xDifference);
            }
        }

        private static readonly float RotationSpeed = MathHelper.ToRadians(0.2f);

        [CanBeNull]
        private Camera _camera;
        private MouseState _lastMoustState;

    }
}

