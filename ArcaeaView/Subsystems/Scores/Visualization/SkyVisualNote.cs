using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;
using OpenMLTD.MilliSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="DisposableBase" />
    /// <inheritdoc cref="IPreviewNote"/>
    /// <inheritdoc cref="IDrawableNote"/>
    /// <summary>
    /// Represents a sky visual note.
    /// </summary>
    public sealed class SkyVisualNote : DisposableBase, IPreviewNote, IDrawableNote {

        public SkyVisualNote([NotNull] VisualBeatmap beatmap, [NotNull] SkyNote baseNote, [NotNull] StageMetrics metrics) {
            _metrics = metrics;
            BaseNote = baseNote;
            PreviewY = beatmap.CalculateY(baseNote.Tick, metrics, metrics.FinishLineY);

            _noteBox = new ColoredBox(beatmap.GraphicsDevice);
        }

        public SkyNote BaseNote { get; }

        public float PreviewY { get; }

        public bool IsVisible(int beatmapTicks, float currentY) {
            if (BaseNote.Tick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < BaseNote.Tick) {
                return false;
            }

            return !(PreviewY < currentY || currentY + _metrics.TrackLength < PreviewY);
        }

        public void SetVertices(Vector3 bottomNearLeft, Vector3 size) {
            _noteBox.SetVertices(bottomNearLeft, size, TranslucentLightBlue);
        }

        public void Draw(Effect effect, int beatmapTicks, float currentY) {
            _noteBox.Draw(effect.CurrentTechnique);
        }

        protected override void Dispose(bool disposing) {
            _noteBox?.Dispose();
            _noteBox = null;
        }

        private static readonly Color TranslucentLightBlue = new Color(Color.LightBlue, 0.4f);

        private readonly StageMetrics _metrics;
        private ColoredBox _noteBox;

    }
}
