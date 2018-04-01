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
            var left = ((int)_baseNote.Track - 0.5f) * metrics.TrackInnerWidth / 4 - metrics.FloorNoteWidth / 2 - metrics.HalfTrackInnerWidth;
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

                // var skyPoint = new Vector3(skyMiddle, skyBottom, skyEdge);
                // Optimization for sky point calculation needed

                var ratio = (float)(((SkyNote)synced.BaseNote).Tick - n.StartTick) / (n.EndTick - n.StartTick);

                var startX = (n.StartX - -0.5f) * metrics.TrackInnerWidth / (1.5f - -0.5f) - metrics.HalfTrackInnerWidth;
                var startZ = metrics.SkyInputZ * n.StartY + metrics.ArcHeightLowerBorder * (1 - n.StartY);

                var endX = (n.EndX - -0.5f) * metrics.TrackInnerWidth / (1.5f - -0.5f) - metrics.HalfTrackInnerWidth;
                var endZ = metrics.SkyInputZ * n.EndY + metrics.ArcHeightLowerBorder * (1 - n.EndY);

                var start = new Vector3(startX, 1, startZ);
                var end = new Vector3(endX, 1, endZ);

                var skyPoint = ArcEasingHelper.Ease(start, end, ratio, n.Easing);
                skyPoint.Y = synced.PreviewY - currentY;

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
