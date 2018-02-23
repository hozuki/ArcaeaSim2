using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <summary>
    /// Vertex format, containing a position element, a normal element, and a color element.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PosNormalColor {

        public Vector3 Position;

        public Vector3 Normal;

        public Vector4 Color;

        private static readonly VertexElement[] VertexElements = {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
        };

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(VertexElements);

    }
}
