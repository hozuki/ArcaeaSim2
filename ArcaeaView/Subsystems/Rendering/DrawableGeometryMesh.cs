using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <inheritdoc />
    /// <summary>
    /// Represents a drawable mesh of a simple geometry.
    /// </summary>
    public abstract class DrawableGeometryMesh : DisposableBase {

        /// <summary>
        /// Draws this mesh. Be sure to have its vertex buffer and index buffer ready before calling.
        /// </summary>
        /// <param name="technique">The <see cref="EffectTechnique"/> to use to draw this mesh.</param>
        public abstract void Draw([NotNull] EffectTechnique technique);

    }
}
