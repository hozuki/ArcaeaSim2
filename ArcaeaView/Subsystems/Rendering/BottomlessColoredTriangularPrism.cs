using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Uses <see cref="VertexPositionColor" />.
    /// </summary>
    public sealed class BottomlessColoredTriangularPrism : DrawableGeometryMesh {

        public BottomlessColoredTriangularPrism([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, 6, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 12, BufferUsage.WriteOnly);

            var indices = new ushort[] {
                // Arcs have only two sides.
                0, 1, 3,
                3, 1, 4,
                0, 2, 3,
                3, 2, 5
            };

            _indexBuffer.SetData(indices);
        }

        public override void Draw(EffectTechnique technique) {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            foreach (var pass in technique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
            }
        }

        public void SetVerticesXY(Vector3 start, Vector3 end, Color color, float rightAngleSideWidth) {
            // Half of the width of the right angle side
            var std = rightAngleSideWidth / 2;
            var dx = std;
            var dzDown = std / 2;
            var dzUp = dzDown;

            var vertices = new[] {
                new VertexPositionColor {Position = new Vector3(start.X, start.Y, start.Z + dzUp), Color = color},
                new VertexPositionColor {Position = new Vector3(start.X - dx, start.Y, start.Z - dzDown), Color = color},
                new VertexPositionColor {Position = new Vector3(start.X + dx, start.Y, start.Z - dzDown), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X, end.Y, end.Z + dzUp), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X - dx, end.Y, end.Z - dzDown), Color = color},
                new VertexPositionColor {Position = new Vector3(end.X + dx, end.Y, end.Z - dzDown), Color = color}
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
