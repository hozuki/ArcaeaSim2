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

        public void SetVerticesXY(Vector2 origin, Vector2 size, Color color, float z) {
            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(1, 0)}
            };

            _vertexBuffer.SetData(vertices);
        }

        public void SetVerticesXZ(Vector2 origin, Vector2 size, Color color, float y) {
            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(origin.X, y, origin.Y), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, y, origin.Y), Color = color, TextureCoordinate = new Vector2(1, 1)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X, y, origin.Y + size.Y), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, y, origin.Y + size.Y), Color = color, TextureCoordinate = new Vector2(1, 0)}
            };

            _vertexBuffer.SetData(vertices);
        }

        public void SetVerticesXY(Vector2 origin, Vector2 size, Color color, float textureYOffset, float z) {
            var y = size.Y + textureYOffset;

            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(0, y)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y, z), Color = color, TextureCoordinate = new Vector2(1, y)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(0, textureYOffset)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, origin.Y + size.Y, z), Color = color, TextureCoordinate = new Vector2(1, textureYOffset)}
            };

            _vertexBuffer.SetData(vertices);
        }

        // Rotated, counterclockwise, 90deg
        public void SetVerticesXZTextureRotated(Vector2 origin, Vector2 size, Color color, float y) {
            var vertices = new[] {
                new VertexPositionColorTexture {Position = new Vector3(origin.X, y, origin.Y), Color = color, TextureCoordinate = new Vector2(0, 0)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, y, origin.Y), Color = color, TextureCoordinate = new Vector2(0, 1)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X, y, origin.Y + size.Y), Color = color, TextureCoordinate = new Vector2(1, 0)},
                new VertexPositionColorTexture {Position = new Vector3(origin.X + size.X, y, origin.Y + size.Y), Color = color, TextureCoordinate = new Vector2(1, 1)}
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
