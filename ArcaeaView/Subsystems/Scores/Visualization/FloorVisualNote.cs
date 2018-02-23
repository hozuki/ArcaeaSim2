using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="VisualNoteBase" />
    /// <inheritdoc cref="IPreviewNote"/>
    /// <summary>
    /// Represents a floor visual note.
    /// </summary>
    public sealed class FloorVisualNote : VisualNoteBase, IPreviewNote {

        public FloorVisualNote([NotNull] VisualBeatmap beatmap, [NotNull] FloorNote baseNote, [NotNull] StageMetrics metrics)
            : base(beatmap, baseNote) {
            _baseNote = baseNote;
            _metrics = metrics;
            _noteRectangle = new TexturedRectangle(beatmap.GraphicsDevice);

            PreviewY = beatmap.CalculateY(baseNote.Tick, metrics, metrics.FinishLineY);
        }

        public override void Draw(int beatmapTicks, float currentY) {
            var left = ((int)_baseNote.Track - 1) * _metrics.TrackInnerWidth / 4 - _metrics.HalfTrackInnerWidth;
            var bottom = PreviewY - currentY;

            var bottomLeft = new Vector2(left, bottom);
            var size = new Vector2(_metrics.FloorNoteWidth, _metrics.FloorNoteHeight);

            _noteRectangle.SetVerticesXY(bottomLeft, size, Color.White, Z);

            var effect = (BasicEffect)NoteEffects.Effects[(int)_baseNote.Type];

            effect.Alpha = 0.75f;

            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

            effect.Texture = _texture;

            _noteRectangle.Draw(effect.CurrentTechnique);
        }

        public override bool IsVisible(int beatmapTicks, float currentY) {
            if (_baseNote.Tick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < _baseNote.Tick) {
                return false;
            }

            return !(PreviewY < currentY || currentY + _metrics.TrackLength < PreviewY);
        }

        public void SetTexture([NotNull] Texture2D texture) {
            _texture = texture;
        }

        public float PreviewY { get; }

        protected override void Dispose(bool disposing) {
            _noteRectangle?.Dispose();
            _noteRectangle = null;
        }

        private const float Z = 0.04f;

        private Texture2D _texture;

        private readonly FloorNote _baseNote;
        private readonly StageMetrics _metrics;
        private TexturedRectangle _noteRectangle;

    }
}
