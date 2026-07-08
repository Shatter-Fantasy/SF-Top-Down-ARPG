using System.Collections.Generic;
using Unity.Collections;
using Unity.U2D.Physics;
using UnityEngine;

namespace SF.U2D.Physics
{
    public class SFPhysicsCulling
    {
        public List<PhysicsCullingOperation> CullingOperations = new ();


        public void StartCulling()
        {
            foreach (var cullingOperation in CullingOperations)
            {
                cullingOperation.CullPhysicsBodies();
            }
        }
    }


    /// <summary>
    /// Takes inspiration from the RenderGraph passes for passing in pass data.
    /// </summary>
    public class PhysicsCullingOperation
    {
        public virtual void CullPhysicsBodies() { }
    }

    /// <summary>
    /// <example>Have a PhysicsCullingOperation that takes in a data strict with information of center and distance to center to cull physics bodies to far away.</example>
    /// </summary>
    /// <typeparam name="TCullingData">Any data struct to be used for doing calculations for if the PhysicsBodies need to be culled or not.</typeparam>
    public abstract class PhysicsCullingOperation<TCullingData> : PhysicsCullingOperation where TCullingData : struct
    {
        public TCullingData CullingData;
    }

    public class FarDistanceCullingOperation : PhysicsCullingOperation<FarDistanceData>
    {

        public float FarDistanceDefault;

        public override void CullPhysicsBodies()
        {
            CleanUpFarDistanceBodies();
        }

        public FarDistanceCullingOperation(float farDistanceDefault )
        {
            if (farDistanceDefault <= 0)
                farDistanceDefault = 50;

            FarDistanceDefault = farDistanceDefault;
        }


        public void CleanUpFarDistanceBodies(float farDistance = 0)
        {
            farDistance = (farDistance == 0) ? FarDistanceDefault : farDistance;
            CleanUpFarDistanceBodies(SFPhysicsManager.SimulationCenter, farDistance);
        }

        public void CleanUpFarDistanceBodies(in Vector2 center,in float farDistance)
        {
            ref NativeList<PhysicsBody> activeBodies = ref SFPhysicsManager.ActiveBodies;

            if(!activeBodies.IsCreated || activeBodies.Count < 1)
                return;

            NativeList<PhysicsBody> tempBodies = new NativeList<PhysicsBody>(0,Allocator.Temp);
            tempBodies.CopyFrom(activeBodies);
            foreach (var activeBody in tempBodies)
            {
                if (!activeBody.isValid)
                {
                    activeBodies.RemoveAt(activeBodies.IndexOf(activeBody));
                }
                else
                {
                    if (Vector2.Distance(activeBody.position, center) > farDistance)
                    {
                        activeBodies.RemoveAt(activeBodies.IndexOf(activeBody));
                        activeBody.Destroy();
                    }
                }
            }

            tempBodies.Dispose();
        }
    }

    public struct FarDistanceData
    {
        public Vector2 Center;
        public int Distance;
    }
}