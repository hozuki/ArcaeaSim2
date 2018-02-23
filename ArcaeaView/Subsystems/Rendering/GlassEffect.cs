using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    public sealed class GlassEffect : Effect {

        public GlassEffect(GraphicsDevice graphicsDevice, byte[] effectCode)
            : base(graphicsDevice, effectCode) {
            Initialize();
        }

        public float Opacity {
            get => _opacity.GetValueSingle();
            set => _opacity.SetValue(MathHelper.Clamp(value, 0, 1));
        }

        public Color TintColor {
            get => new Color(_tintColor.GetValueVector4());
            set => _tintColor.SetValue(value.ToVector4());
        }

        public Matrix WorldViewProjection {
            get => _worldViewProjection.GetValueMatrix();
            set => _worldViewProjection.SetValue(value);
        }

        public Matrix World {
            get => _world.GetValueMatrix();
            set => _world.SetValue(value);
        }

        public Matrix WorldInverse {
            get => _worldInv.GetValueMatrix();
            set => _worldInv.SetValue(value);
        }

        public Vector3 CameraPosition {
            get => _cameraPosition.GetValueVector3();
            set => _cameraPosition.SetValue(value);
        }

        public Color EdgeColor {
            get => new Color(_edgeColor.GetValueVector4());
            set => _edgeColor.SetValue(value.ToVector4());
        }

        public float EdgeThickness {
            get => _edgeThickness.GetValueSingle();
            set => _edgeThickness.SetValue(MathF.ClampLower(value, float.Epsilon));
        }

        private void Initialize() {
            var p = Parameters;

            _opacity = p["gOpacity"];
            _tintColor = p["gTintColor"];
            _worldViewProjection = p["gWorldViewProjection"];
            _world = p["gWorld"];
            _worldInv = p["gWorldInv"];
            _cameraPosition = p["gCameraPosition"];
            _edgeColor = p["gEdgeColor"];
            _edgeThickness = p["gEdgeThickness"];

            Opacity = 1;
            TintColor = Color.White;
            EdgeColor = Color.White;
        }

        private EffectParameter _opacity;
        private EffectParameter _tintColor;
        private EffectParameter _worldViewProjection;
        private EffectParameter _world;
        private EffectParameter _worldInv;
        private EffectParameter _cameraPosition;
        private EffectParameter _edgeColor;
        private EffectParameter _edgeThickness;

    }
}
