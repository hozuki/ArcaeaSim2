using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// <para>Represents a colored box. A box is defined by its bottom left back corner (near to the X axis) and its size on X, Y and Z axes. Its faces are parallel to the axes.</para>
    /// <para>Uses vertex format <see cref="PosColor" />.</para>
    /// </summary>
    public sealed class ColoredBox : DrawableGeometryMesh {

        /// <summary>
        /// Creates a new <see cref="ColoredBox"/> instance.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/> to use for rendering the box.</param>
        public ColoredBox([NotNull] GraphicsDevice graphicsDevice) {
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

        /// <summary>
        /// Set and update mesh vertices.
        /// </summary>
        /// <param name="bottomNearLeft">The bottom left back point.</param>
        /// <param name="size">Size of this box.</param>
        /// <param name="color">The color of this box.</param>
        public void SetVertices(Vector3 bottomNearLeft, Vector3 size, Color color) {
            var colorf = color.ToVector4();

            var vertices = new[] {
                new PosColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y, bottomNearLeft.Z + size.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf},
                new PosColor {Position = new Vector3(bottomNearLeft.X + size.X, bottomNearLeft.Y + size.Y, bottomNearLeft.Z + size.Z), Color = colorf}
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
