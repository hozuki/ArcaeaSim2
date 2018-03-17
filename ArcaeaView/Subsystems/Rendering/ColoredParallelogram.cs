using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="VertexPositionColor" />.
    /// </summary>
    public sealed class ColoredParallelogram : DrawableGeometryMesh {

        public ColoredParallelogram([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

            var indices = new ushort[] {
                0, 1, 2,
                2, 1, 3
            };

            _indexBuffer.SetData(indices);
        }

        public override void Draw(EffectTechnique technique) {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
        }

        public void SetVerticesXY(Vector2 start, Vector2 end, float width, Color color, float z = 0) {
            var angle = MathF.Atan2(end.Y - start.Y, end.X - start.X);
            var dx = width / 2 * MathF.Sin(angle);
            var dy = width / 2 * MathF.Cos(angle);

            var vertices = new[] {
                new VertexPositionColor {Position = new Vector3(start.X - dx, start.Y - dy, z), Color = color},
                new VertexPositionColor {Position = new Vector3(start.X + dx, start.Y + dy, z), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X - dx, end.Y - dy, z), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X + dx, end.Y + dy, z), Color = color}
            };

            _vertexBuffer.SetData(vertices);
        }

        public void SetVerticesXYParallel(Vector2 start, Vector2 end, float width, Color color, float z = 0) {
            var halfWidth = width / 2;

            var vertices = new[] {
                new VertexPositionColor {Position = new Vector3(start.X - halfWidth, start.Y, z), Color = color},
                new VertexPositionColor {Position = new Vector3(start.X + halfWidth, start.Y, z), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X - halfWidth, end.Y, z), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X + halfWidth, end.Y, z), Color = color}
            };

            _vertexBuffer.SetData(vertices);
        }

        protected override void Dispose(bool disposing) {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            _vertexBuffer = null;
            _indexBuffer = null;
        }

        private readonly GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

    }
}
