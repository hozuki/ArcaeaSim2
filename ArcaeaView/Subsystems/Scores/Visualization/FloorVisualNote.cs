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
            _linkHexahedron = new ColoredHexahedron(beatmap.GraphicsDevice);

            PreviewY = beatmap.CalculateY(baseNote.Tick, metrics, metrics.FinishLineY);
        }

        public override void Draw(int beatmapTicks, float currentY) {
            var metrics = _metrics;
            var left = ((int)_baseNote.Track - 1) * metrics.TrackInnerWidth / 4 - metrics.HalfTrackInnerWidth;
            var bottom = PreviewY - currentY;

            var bottomLeft = new Vector2(left, bottom);
            var size = new Vector2(metrics.FloorNoteWidth, metrics.FloorNoteHeight);

            _noteRectangle.SetVerticesXY(bottomLeft, size, Color.White, Z);

            var effect = (BasicEffect)NoteEffects.Effects[(int)_baseNote.Type];

            effect.Alpha = 0.75f;

            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

            effect.Texture = _texture;

            _noteRectangle.Draw(effect.CurrentTechnique);

            var synced = SynchronizedSkyVisualNote;

            if (synced != null) {
                // Draw the sync line.

                var n = (ArcNote)synced.Parent.BaseNote;

                // TODO: This calculation can be executed when loading the beatmap, and store in some fields like "PreviewX".
                var ratio = (float)(((SkyNote)synced.BaseNote).Tick - n.StartTick) / (n.EndTick - n.StartTick);

                var xRatio = MathHelper.Lerp(n.StartX, n.EndX, ratio);
                var yRatio = MathHelper.Lerp(n.StartY, n.EndY, ratio);

                var skyMiddle = (xRatio - -0.5f) * metrics.TrackInnerWidth / (1.5f - -0.5f) - metrics.HalfTrackInnerWidth;
                var skyBottom = synced.PreviewY - currentY;
                var skyEdge = metrics.SkyInputZ * yRatio - metrics.SkyNoteTallness / 2;

                var skyPoint = new Vector3(skyMiddle, skyBottom, skyEdge);
                var thisPoint = new Vector3(left + metrics.FloorNoteWidth / 2, bottom, Z);

                _linkHexahedron.SetVerticesXZ(thisPoint, skyPoint, Color.PaleVioletRed, LinkSectionSize);

                effect.Alpha = 0.2f;

                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;

                _linkHexahedron.Draw(effect.CurrentTechnique);
            }
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

        [CanBeNull]
        public SkyVisualNote SynchronizedSkyVisualNote { get; internal set; }

        protected override void Dispose(bool disposing) {
            _noteRectangle?.Dispose();
            _noteRectangle = null;
            _linkHexahedron?.Dispose();
            _linkHexahedron = null;
        }

        private const float Z = 0.04f;
        private static readonly Vector2 LinkSectionSize = new Vector2(0.25f, 0.25f);

        private Texture2D _texture;

        private readonly FloorNote _baseNote;
        private readonly StageMetrics _metrics;
        private TexturedRectangle _noteRectangle;
        private ColoredHexahedron _linkHexahedron;

    }
}
