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
            _coloredRectangle = new ColoredRectangle(beatmap.GraphicsDevice);

            PreviewStartY = beatmap.CalculateY(baseNote.StartTick, metrics, -metrics.FloorNoteHeight / 2 + metrics.FinishLineY);
            PreviewEndY = beatmap.CalculateY(baseNote.EndTick, metrics, -metrics.FloorNoteHeight / 2 + metrics.FinishLineY);
        }

        public override void Draw(Effect effect, int beatmapTicks, float currentY) {
            var left = ((int)_baseNote.Track - 1) * _metrics.TrackInnerWidth / 4 - _metrics.HalfTrackInnerWidth;
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

            _coloredRectangle.SetVertices(bottomLeft, size, TranslucentWhite, Z);

            _coloredRectangle.Draw(effect.CurrentTechnique);
        }

        public override bool IsVisible(int beatmapTicks, float currentY) {
            if (_baseNote.EndTick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < _baseNote.StartTick) {
                return false;
            }

            return !(PreviewEndY < currentY || currentY + _metrics.TrackLength < PreviewStartY);
        }

        public float PreviewStartY { get; }

        public float PreviewEndY { get; }

        protected override void Dispose(bool disposing) {
            _coloredRectangle?.Dispose();
            _coloredRectangle = null;
        }

        private static readonly Color TranslucentWhite = new Color(Color.White, 0.75f);
        private const float Z = 0.04f;

        private readonly LongNote _baseNote;
        private readonly StageMetrics _metrics;
        private ColoredRectangle _coloredRectangle;

    }
}
