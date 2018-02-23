using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    public sealed class VertexColorEffect : Effect {

        public VertexColorEffect([NotNull] GraphicsDevice graphicsDevice, [NotNull] byte[] effectCode)
            : base(graphicsDevice, effectCode) {
            Initialize();
        }

        public Matrix WorldViewProjection {
            get => _worldViewProjection.GetValueMatrix();
            set => _worldViewProjection.SetValue(value);
        }

        public float Opacity {
            get => _opacity.GetValueSingle();
            set => _opacity.SetValue(MathHelper.Clamp(value, 0, 1));
        }

        public Color TintColor {
            get => new Color(_tintColor.GetValueVector4());
            set => _tintColor.SetValue(value.ToVector4());
        }

        private void Initialize() {
            var p = Parameters;

            _worldViewProjection = p["gWorldViewProjection"];
            _opacity = p["gOpacity"];
            _tintColor = p["gTintColor"];

            Opacity = 1;
            TintColor = Color.White;
        }

        private EffectParameter _worldViewProjection;
        private EffectParameter _opacity;
        private EffectParameter _tintColor;

    }
}
