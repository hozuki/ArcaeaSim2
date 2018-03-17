using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Actually front-less and back-less (LOL).
    /// Uses <see cref="VertexPositionColor" />.
    /// </summary>
    public sealed class BottomlessTexturedTetrahedron : DrawableGeometryMesh {

        public BottomlessTexturedTetrahedron([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 9, BufferUsage.WriteOnly);

            var indices = new ushort[] {
                0, 1, 2,
                0, 1, 3
            };

            _indexBuffer.SetData(indices);
        }

        public override void Draw(EffectTechnique technique) {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 3);
            }
        }

        public void SetVerticesXZ(Vector3 point, Color color, float rightAngleSideWidth) {
            var std = rightAngleSideWidth / 2;
            var dx = std;
            var dy = std;
            var dzDown = std / 2;
            var dzUp = dzDown;

            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(point.X, point.Y, point.Z + dzUp), Color = color, TextureCoordinate = new Vector2(1, 0)},
                new VertexPositionColorTexture {Position = new Vector3(point.X, point.Y - dy, point.Z - dzDown), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(point.X - dx, point.Y, point.Z - dzDown), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(point.X + dx, point.Y, point.Z - dzDown), Color = color, TextureCoordinate = new Vector2(0, 0)},
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
