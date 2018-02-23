using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="PosColor" />.
    /// </summary>
    public sealed class ColoredRectangle : DrawableGeometryMesh {

        public ColoredRectangle([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, PosColor.VertexDeclaration, 4, BufferUsage.WriteOnly);
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

        public void SetVertices(Vector2 bottomLeft, Vector2 size, Color color, float z = 0) {
            var colorf = color.ToVector4();

            var vertices = new[] {
                new PosColor {Position = new Vector3(bottomLeft.X, bottomLeft.Y, z), Color = colorf},
                new PosColor {Position = new Vector3(bottomLeft.X + size.X, bottomLeft.Y, z), Color = colorf},
                new PosColor {Position = new Vector3(bottomLeft.X, bottomLeft.Y + size.Y, z), Color = colorf},
                new PosColor {Position = new Vector3(bottomLeft.X + size.X, bottomLeft.Y + size.Y, z), Color = colorf}
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
