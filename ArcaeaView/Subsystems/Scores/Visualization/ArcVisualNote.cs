using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Extensions;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="VisualNoteBase" />
    /// <inheritdoc cref="IRangedPreviewNote"/>
    /// <summary>
    /// Represents an arc visual note.
    /// </summary>
    public sealed class ArcVisualNote : VisualNoteBase, IRangedPreviewNote {

        public ArcVisualNote([NotNull] VisualBeatmap beatmap, [NotNull] ArcNote baseNote, [NotNull] StageMetrics metrics)
            : base(beatmap, baseNote) {
            _baseNote = baseNote;
            _metrics = metrics;

            PreviewStartY = beatmap.CalculateY(baseNote.StartTick, metrics, metrics.FinishLineY);
            PreviewEndY = beatmap.CalculateY(baseNote.EndTick, metrics, metrics.FinishLineY);

            _arcMesh = new BottomlessTexturedTriangularPrism(beatmap.GraphicsDevice);
            _shadow = new ColoredParallelogram(beatmap.GraphicsDevice);
            _support = new TexturedRectangle(beatmap.GraphicsDevice);
            _header = new BottomlessTexturedTetrahedron(beatmap.GraphicsDevice);

            if (baseNote.SkyNotes != null && baseNote.SkyNotes.Length > 0) {
                SkyVisualNotes = baseNote.SkyNotes.Select(n => new SkyVisualNote(beatmap, n, this, metrics)).ToArray();
            }
        }

        public override void Draw(int beatmapTicks, float currentY) {
            var metrics = _metrics;
            var skyNotes = SkyVisualNotes;
            var n = _baseNote;

            var startX = (n.StartX - -0.5f) * metrics.TrackInnerWidth / (1.5f - -0.5f) - metrics.HalfTrackInnerWidth;
            var startY = PreviewStartY - currentY;
            var startZ = metrics.SkyInputZ * n.StartY + metrics.PlayableArcTallness * (1 - n.StartY) / 4;

            var endX = (n.EndX - -0.5f) * metrics.TrackInnerWidth / (1.5f - -0.5f) - metrics.HalfTrackInnerWidth;
            var endY = PreviewEndY - currentY;
            var endZ = metrics.SkyInputZ * n.EndY + metrics.PlayableArcTallness * (1 - n.EndY) / 4;

            var start = new Vector3(startX, startY, startZ);
            var end = new Vector3(endX, endY, endZ);

            if (skyNotes?.Length > 0){
                var skyNoteSize = new Vector3(metrics.SkyNoteWidth, metrics.SkyNoteHeight, metrics.SkyNoteTallness);

                foreach (var skyNote in skyNotes) {
                    if (!skyNote.IsVisible(beatmapTicks, currentY)) {
                        continue;
                    }

                    var ratio = (float)(((SkyNote)skyNote.BaseNote).Tick - n.StartTick) / (n.EndTick - n.StartTick);

                    //var bottomNearLeft = new Vector3(left, bottom, corner);

                    var bottomNearLeft = ArcEasingHelper.Ease(start, end, ratio, n.Easing);

                    bottomNearLeft.X -= metrics.SkyNoteWidth / 2;
                    bottomNearLeft.Z -= metrics.SkyNoteTallness / 2;

                    skyNote.SetVertices(bottomNearLeft, skyNoteSize);

                    skyNote.SetTexture(_skyVisualNoteTexture);

                    skyNote.Draw(beatmapTicks, currentY);
                }
            }


            Color arcColor;
            Vector2 arcSectionSize;
            float alpha;
            bool castShadow;

            if (n.IsPlayable) {
                arcColor = n.Color == ArcColor.Magenta ? RedArc : BlueArc;
                arcSectionSize = new Vector2(metrics.PlayableArcWidth, metrics.PlayableArcTallness);
                alpha = 0.75f;
                castShadow = true;
            } else {
                arcColor = TraceArc;
                arcSectionSize = new Vector2(metrics.TraceArcWidth, metrics.TraceArcTallness);
                alpha = 0.5f;
                castShadow = false;
            }

            var segmentCount = n.EndTick - n.StartTick > 1000 ? 14 : 7;
            var effect = (BasicEffect)NoteEffects.Effects[(int)n.Type];

            DrawArc(effect, segmentCount, start, end, n.Easing, arcColor, alpha, arcSectionSize, castShadow);

            if (n.IsPlayable) {
                if (ShouldDrawHeader && beatmapTicks <= n.StartTick) {
                    _header.SetVerticesXZ(start, arcColor, arcSectionSize.X);

                    effect.TextureEnabled = true;
                    effect.VertexColorEnabled = true;

                    effect.Texture = _arcVisualNoteTexture;

                    effect.Alpha = alpha;

                    _header.Draw(effect.CurrentTechnique);
                }

                if (ShouldDrawSupport && startZ > 0 && beatmapTicks <= n.StartTick) {
                    var supportBottom = new Vector2(startX - metrics.PlayableArcWidth / 2, 0);
                    var supportSize = new Vector2(metrics.PlayableArcWidth, startZ);

                    _support.SetVerticesXZTextureRotated(supportBottom, supportSize, arcColor, startY);

                    effect.TextureEnabled = true;
                    effect.VertexColorEnabled = true;

                    effect.Alpha = 0.8f;
                    effect.Texture = _supportTexture;

                    _support.Draw(effect.CurrentTechnique);
                }
            }
        }

        public override bool IsVisible(int beatmapTicks, float currentY) {
            if (_baseNote.EndTick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < _baseNote.StartTick) {
                return false;
            }

            return !(PreviewEndY < currentY || currentY + _metrics.TrackLength < PreviewStartY);
        }

        public float PreviewStartY { get; }

        public float PreviewEndY { get; }

        public bool ShouldDrawSupport { get; internal set; }

        public bool ShouldDrawHeader { get; internal set; }

        public void SetSkyNoteTexture([NotNull] Texture2D texture) {
            _skyVisualNoteTexture = texture;
        }

        public void SetSupportTexture([NotNull] Texture2D texture) {
            _supportTexture = texture;
        }

        public void SetArcTexture([NotNull]Texture2D texture){
            _arcVisualNoteTexture = texture;
        }

        /// <summary>
        /// Gets the <see cref="SkyNote"/>s associated with this note.
        /// </summary>
        [CanBeNull]
        public SkyVisualNote[] SkyVisualNotes { get; }

        protected override void Dispose(bool disposing) {
            if (SkyVisualNotes != null) {
                foreach (var skyNote in SkyVisualNotes) {
                    skyNote.Dispose();
                }
            }

            _arcMesh?.Dispose();
            _arcMesh = null;

            _shadow?.Dispose();
            _shadow = null;

            _support?.Dispose();
            _support = null;

            _header?.Dispose();
            _header = null;
        }

        private void DrawArc([NotNull] BasicEffect effect, int segmentCount, Vector3 start, Vector3 end, ArcEasing easing, Color color, float alpha, Vector2 arcSectionSize, bool castShadow) {
            var lastPoint = start;
            var zeroY = _metrics.FinishLineY;
            var trackEndY = _metrics.TrackLength;

            for (var i = 1; i <= segmentCount; ++i) {
                if (lastPoint.Y > trackEndY) {
                    // This segment and later segments have not entered the track yet.
                    break;
                }

                Vector3 currentPoint;

                if (i == segmentCount) {
                    currentPoint = end;
                } else {
                    var ratio = (float)i / segmentCount;
                    currentPoint = ArcEasingHelper.Ease(start, end, ratio, easing);
                }

                if (lastPoint.Y < zeroY && currentPoint.Y < zeroY) {
                    // This segment has passed.
                    continue;
                }

                Vector3 fixedLastPoint, fixedCurrentPoint;

                // Recalculate the segment's start and end if needed.
                // However, we must ensure that the movement of the intersection of the arc and XoZ plane is always continuous,
                // therefore we must call the easing function again to retrieve its precise new location, instead of learping
                // inside the segment's start and end (shows recognizable "shaking" effect).
                // Credit: @18111398
                if (lastPoint.Y < zeroY) {
                    var ratio = (zeroY - start.Y) / (end.Y - start.Y);
                    fixedLastPoint = ArcEasingHelper.Ease(start, end, ratio, easing);
                } else {
                    fixedLastPoint = lastPoint;
                }

                if (currentPoint.Y > trackEndY) {
                    var ratio = (trackEndY - start.Y) / (end.Y - start.Y);
                    fixedCurrentPoint = ArcEasingHelper.Ease(start, end, ratio, easing);
                } else {
                    fixedCurrentPoint = currentPoint;
                }

                _arcMesh.SetVerticesXY(fixedLastPoint, fixedCurrentPoint, color, arcSectionSize.X);

                effect.TextureEnabled = true;
                effect.VertexColorEnabled = true;

                effect.Texture = _arcVisualNoteTexture;

                effect.Alpha = alpha;

                _arcMesh.Draw(effect.CurrentTechnique);

                // Draw shadow if needed.
                if (castShadow) {
                    _shadow.SetVerticesXYParallel(fixedLastPoint.XY(), fixedCurrentPoint.XY(), arcSectionSize.X, Color.Gray, ShadowZ);

                    effect.TextureEnabled = false;
                    effect.VertexColorEnabled = true;

                    effect.Alpha = 0.5f;

                    _shadow.Draw(effect.CurrentTechnique);
                }

                // Update the point.
                lastPoint = currentPoint;
            }
        }

        private const float ShadowZ = 0.03f;

        private Texture2D _skyVisualNoteTexture;
        private Texture2D _supportTexture;
        private Texture2D _arcVisualNoteTexture;

        private static readonly Color RedArc = new Color(220, 151, 193).ChangeBrightness(-40);
        private static readonly Color ErrorArc = new Color(0xe6, 0x32, 0x32);
        private static readonly Color BlueArc = Color.DeepSkyBlue.ChangeBrightness(-80);
        private static readonly Color TraceArc = new Color(80, 67, 104).ChangeBrightness(30);

        private readonly ArcNote _baseNote;
        private readonly StageMetrics _metrics;
        private BottomlessTexturedTriangularPrism _arcMesh;
        private ColoredParallelogram _shadow;
        private TexturedRectangle _support;
        private BottomlessTexturedTetrahedron _header;

    }
}
