using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.U2D.Physics;

namespace SF.U2D.Physics
{
    [BurstCompile]
    public static class PhysicsBodyExtensions
    {
        public static bool TryGetCallbackComponent<T>(this PhysicsBody body,out T component, bool checkShapeValidation = false)
        {
            component = (T)default;
            
            // Optional check for only using Component set as a callbackTarget for valid PhysicsBody.
            if (checkShapeValidation && !body.isValid)
                return false;

            if (body.callbackTarget is not T callbackTarget)
            {
                if (body.callbackTarget is not Component callbackComponent ||
                    !callbackComponent.TryGetComponent(out component))
                    return false;
            }
            
            return true;
        }
        
        public static bool TryGetCallbackShapeComponent<T>(this PhysicsBody body,out T component, bool checkShapeValidation = false) where T : SFShapeComponent
        {
            component = null;
            
            // Optional check for only using Component set as a callbackTarget for valid PhysicsBody.
            if (checkShapeValidation && !body.isValid)
                return false;

            if (body.callbackTarget is not T callbackTarget) 
                return false;
            
            component = callbackTarget;
            return true;
        }

        
        [BurstCompile]
        public static void GetDirectionToNormalized(in this PhysicsBody fromBody, in PhysicsBody toBody, ref Vector2 direction)
        {
            if (!fromBody.isValid || !toBody.isValid)
                return;
            direction = (fromBody.position - toBody.position).normalized;
        }
        
        [BurstCompile]
        public static void GetDirectionToNormalized(in this PhysicsBody fromBody, in PhysicsShape toShape, ref Vector2 direction)
        {
            if (!fromBody.isValid || !toShape.isValid || !toShape.body.isValid)
                return;
            direction = (fromBody.position - toShape.body.position).normalized;
        }
        
        [BurstCompile]
        public static void GetDirectionToNormalized(in Vector2 fromPosition, in Vector2 toPosition, ref Vector2 direction)
        {
            direction = (fromPosition - toPosition).normalized;
        }

        public static void CreateShapeBatchWithEqualRotation(ref NativeArray<PhysicsBody> physicsBodies,  ref PhysicsShapeDefinition shapeDefinition)
        {
            float angleDelta = ((float)360 / physicsBodies.Length);
            for (int i = 0; i < physicsBodies.Length; i++)
            {
                PhysicsBody body = physicsBodies[i];
                body.rotation = PhysicsRotate.FromDegrees(angleDelta * i);
                body.linearVelocity = new Vector2(4f,4f) * body.rotation.direction;
                //physicsBodies[i].CreateShape(CircleGeometry,shapeDefinition);
            }
        }
    }
}