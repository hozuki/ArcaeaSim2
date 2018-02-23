using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="VertexPositionColorTexture" />.
    /// </summary>
    public sealed class TexturedRectangle : DrawableGeometryMesh {

        public TexturedRectangle([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
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

        public void SetVertices(Vector2 origin, Vector2 size, Color color, bool normalizeTextureY = false, float z = 0) {
            var textureY = normalizeTextureY ? 1.0f : size.Y;

            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(0, textureY)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(1, textureY)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(1, 0)}
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
