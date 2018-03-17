using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="VisualNoteBase" />
    /// <inheritdoc cref="IRangedPreviewNote"/>
    /// <summary>
    /// Represents a long visual note.
    /// </summary>
    public sealed class LongVisualNote : VisualNoteBase, IRangedPreviewNote {

        public LongVisualNote([NotNull] VisualBeatmap beatmap, [NotNull] LongNote baseNote, [NotNull] StageMetrics metrics)
            : base(beatmap, baseNote) {
            _baseNote = baseNote;
            _metrics = metrics;
            _noteRectangle = new TexturedRectangle(beatmap.GraphicsDevice);

            PreviewStartY = beatmap.CalculateY(baseNote.StartTick, metrics, metrics.FinishLineY);
            PreviewEndY = beatmap.CalculateY(baseNote.EndTick, metrics, metrics.FinishLineY);
        }

        public override void Draw(int beatmapTicks, float currentY) {
            var left = ((int)_baseNote.Track - 0.5f) * _metrics.TrackInnerWidth / 4 - _metrics.FloorNoteWidth / 2 - _metrics.HalfTrackInnerWidth;
            var bottom = PreviewStartY - currentY;

            if (bottom < _metrics.FinishLineY) {
                bottom = _metrics.FinishLineY;
            }

            var top = PreviewEndY - currentY;

            if (top > _metrics.TrackLength) {
                top = _metrics.TrackLength;
            }

            var bottomLeft = new Vector2(left, bottom);
            var size = new Vector2(_metrics.FloorNoteWidth, top - bottom);

            _noteRectangle.SetVerticesXY(bottomLeft, size, Color.White, Z);

            var effect = (BasicEffect)NoteEffects.Effects[(int)_baseNote.Type];

            effect.Alpha = 0.75f;

            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

            // Highlighted before the time reaches this note's start tick, and dim this note after that.
            // Correction credit: @18111398
            var isHighlighted = beatmapTicks >= _baseNote.StartTick;
            var texture = isHighlighted ? _hightlightedTexture : _texture;

            effect.Texture = texture;

            _noteRectangle.Draw(effect.CurrentTechnique);
        }

        public override bool IsVisible(int beatmapTicks, float currentY) {
            if (_baseNote.EndTick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < _baseNote.StartTick) {
                return false;
            }

            return !(PreviewEndY < currentY || currentY + _metrics.TrackLength < PreviewStartY);
        }

        public void SetTextures([NotNull] Texture2D texture, [NotNull] Texture2D highlightedTexture) {
            _texture = texture;
            _hightlightedTexture = highlightedTexture;
        }

        public float PreviewStartY { get; }

        public float PreviewEndY { get; }

        protected override void Dispose(bool disposing) {
            _noteRectangle?.Dispose();
            _noteRectangle = null;
        }

        private const float Z = 0.04f;

        private Texture2D _texture;
        private Texture2D _hightlightedTexture;

        private readonly LongNote _baseNote;
        private readonly StageMetrics _metrics;
        private TexturedRectangle _noteRectangle;

    }
}
