using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc cref="VisualNoteBase"/>
    /// <inheritdoc cref="IPreviewNote"/>
    /// <summary>
    /// Represents a sky visual note.
    /// </summary>
    public sealed class SkyVisualNote : VisualNoteBase, IPreviewNote {

        public SkyVisualNote([NotNull] VisualBeatmap beatmap, [NotNull] SkyNote baseNote, [NotNull] StageMetrics metrics)
        : base(beatmap, baseNote) {
            _metrics = metrics;
            PreviewY = beatmap.CalculateY(baseNote.Tick, metrics, metrics.FinishLineY);

            var graphicsDevice = beatmap.GraphicsDevice;
            _graphicsDevice = graphicsDevice;

            _vertexBuffer1 = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 16, BufferUsage.WriteOnly);
            _indexBuffer1 = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 24, BufferUsage.WriteOnly);

            _vertexBuffer2 = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 8, BufferUsage.WriteOnly);
            _indexBuffer2 = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 12, BufferUsage.WriteOnly);

            var indices1 = new ushort[] {
                // bottom, top
                0, 1, 2,
                2, 1, 3,
                4, 5, 6,
                6, 5, 7,

                // front, back
                8, 9, 10,
                10, 9, 11,
                12, 13, 14,
                14, 13, 15
            };

            _indexBuffer1.SetData(indices1);

            var indices2 = new ushort[] {
                0, 1, 2,
                2, 1, 3,
                4, 5, 6,
                6, 5, 7,
            };

            _indexBuffer2.SetData(indices2);
        }

        public float PreviewY { get; }

        public override bool IsVisible(int beatmapTicks, float currentY) {
            var baseNote = (SkyNote)BaseNote;

            if (baseNote.Tick < beatmapTicks - _metrics.PastTickThreshold || beatmapTicks + _metrics.FutureTickThreshold < baseNote.Tick) {
                return false;
            }

            return !(PreviewY < currentY || currentY + _metrics.TrackLength < PreviewY);
        }

        public override void Draw(int beatmapTicks, float currentY) {
            if (!(NoteEffects.Effects[(int)BaseNote.Type] is BasicEffect effect)) {
                throw new ArgumentException("Sky visual note must use " + nameof(BasicEffect) + ".");
            }

            var technique = effect.CurrentTechnique;


            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            effect.Alpha = 1;

            effect.Texture = _texture1;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer1);
            _graphicsDevice.Indices = _indexBuffer1;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8);
            }

            effect.Texture = _texture2;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer2);
            _graphicsDevice.Indices = _indexBuffer2;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
            }
        }

        public void SetVertices(Vector3 bottomNearLeft, Vector3 size) {
            var color = TranslucentWhite;

            var vertices1 = new[] {
                // bottom
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 0)},

                // up
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 0)},

                // front
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 0)},

                // back
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 0)}
            };

            _vertexBuffer1.SetData(vertices1);

            var vertices2 = new[] {
                // left
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 0)},

                // right
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = color, TextureCoordinate = new Vector2(1, 0)}
            };

            _vertexBuffer2.SetData(vertices2);
        }

        public void SetTexture1([NotNull] Texture2D texture) {
            _texture1 = texture;
        }

        public void SetTexture2([NotNull] Texture2D texture) {
            _texture2 = texture;
        }

        protected override void Dispose(bool disposing) {
            _vertexBuffer1?.Dispose();
            _indexBuffer1?.Dispose();
            _vertexBuffer2?.Dispose();
            _indexBuffer2?.Dispose();
            _vertexBuffer1 = null;
            _indexBuffer1 = null;
            _vertexBuffer2 = null;
            _indexBuffer2 = null;
        }

        private static readonly Color TranslucentLightBlue = new Color(Color.LightBlue, 0.4f);
        private static readonly Color TranslucentWhite = new Color(Color.White, 0.8f);

        private Texture2D _texture1;
        private Texture2D _texture2;

        private readonly StageMetrics _metrics;

        private readonly GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer1;
        private IndexBuffer _indexBuffer1;
        private VertexBuffer _vertexBuffer2;
        private IndexBuffer _indexBuffer2;

    }
}
