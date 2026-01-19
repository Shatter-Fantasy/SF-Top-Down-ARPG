using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Used to tell how custom shapes should be contained inside a <see cref="PhysicsShape"/> that acts like a container.
    /// </summary>
    public interface IPhysicsShapeContained
    {
        /// <summary>
        /// The <see cref="SFShapeComponent"/> that this <see cref="IPhysicsShapeContained"/> is being contained in.
        /// </summary>
        public SFShapeComponent ContainerShape2D { get; set; }

        /// <summary>
        /// Calculates a corrected position to displace an object while keeping it inside the area of a <see cref="PhysicsShape"/>.
        /// </summary>
        /// <returns></returns>
        public Vector2 ContainPosition();
    }
}
