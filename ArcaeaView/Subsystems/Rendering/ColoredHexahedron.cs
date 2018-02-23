using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="PosColor" />.
    /// </summary>
    public sealed class ColoredHexahedron : DrawableGeometryMesh {

        public ColoredHexahedron([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, PosColor.VertexDeclaration, 8, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            var indices = new ushort[] {
                0, 1, 2,
                2, 1, 3,
                4, 5, 6,
                6, 5, 7,

                0, 1, 4,
                1, 4, 5,
                2, 3, 6,
                3, 6, 7,
                0, 2, 4,
                2, 4, 6,
                1, 3, 5,
                3, 5, 7
            };

            _indexBuffer.SetData(indices);
        }

        public override void Draw(EffectTechnique technique) {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }

        public void SetVertices(Vector3 start, Vector3 end, Color color) {
            SetVertices(start, end, color, Vector2.One);
        }

        public void SetVertices(Vector3 start, Vector3 end, Color color, Vector2 sectionSize) {
            var colorf = color.ToVector4();

            var vertices = new[] {
                new PosColor {Position = new Vector3(start.X - sectionSize.X / 2, start.Y, start.Z - sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(start.X + sectionSize.X / 2, start.Y, start.Z - sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(start.X - sectionSize.X / 2, start.Y, start.Z + sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(start.X + sectionSize.X / 2, start.Y, start.Z + sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(end.X - sectionSize.X / 2, end.Y, end.Z - sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(end.X + sectionSize.X / 2, end.Y, end.Z - sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(end.X - sectionSize.X / 2, end.Y, end.Z + sectionSize.Y / 2), Color = colorf},
                new PosColor {Position = new Vector3(end.X + sectionSize.X / 2, end.Y, end.Z + sectionSize.Y / 2), Color = colorf}
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
