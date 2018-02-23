using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="PosNormalColor" />.
    /// </summary>
    public sealed class GlassBox : DrawableGeometryMesh {

        public GlassBox([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, PosNormalColor.VertexDeclaration, 24, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            var indices = new ushort[] {
                0, 1, 2,
                2, 1, 3,
                4, 5, 6,
                6, 5, 7,

                8, 9, 10,
                10, 9, 11,
                12, 13, 14,
                14, 13, 15,
                16, 17, 18,
                18, 17, 19,
                20, 21, 22,
                22, 21, 23
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

        public void SetVertices(Vector3 bottomNearLeft, Vector3 size, Color color) {
            var colorf = color.ToVector4();

            var vertices = new[] {
                // bottom
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitZ},

                // top
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitZ},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitZ},

                // left
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = -Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = -Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitX},

                // right
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = Vector3.UnitX},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = Vector3.UnitX},

                // front
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = -Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf, Normal = -Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = -Vector3.UnitY},

                // back
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf, Normal = Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = Vector3.UnitY},
                new PosNormalColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf, Normal = Vector3.UnitY}
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
