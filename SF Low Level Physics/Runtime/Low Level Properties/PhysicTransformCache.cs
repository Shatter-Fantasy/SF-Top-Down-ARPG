using System.Collections.Generic;
using UnityEngine;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Keeps track of when <see cref="UnityEngine.Transform"/> needs to inform an active <see cref="SFShapeComponent.Body"/>
    /// to update it's <see cref="UnityEngine.LowLevelPhysics2D.PhysicsTransform"/>
    /// </summary>
    public static class PhysicTransformCache
    {
        /// <summary>
        /// Informs any implemented object that a <see cref="Transform"/> has been changed.
        /// </summary>
        public interface ITransformMonitor
        {
            // Need to implement
            void TransformChanged();
        }
            
        private static readonly HashSet<ITransformMonitor> CachedTransform = new ();

        public static void AddMonitor(this ITransformMonitor transform)
        {
            // Need to implement
        }
        
        public static void RemoveMonitor(ITransformMonitor transform)
        {
            // Need to implement
        }
    }
}
