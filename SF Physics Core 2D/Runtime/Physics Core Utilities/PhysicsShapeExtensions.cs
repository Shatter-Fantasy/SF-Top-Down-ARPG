using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.U2D.Physics
{
    [BurstCompile]
    public static class PhysicsShapeExtensions
    {
        
#region PhysicsShape Callbacks Targets
        public static bool TryGetCallbackComponent<T>(this PhysicsShape shape,out T component, bool checkShapeValidation = false) where T : Component
        {
            component = null;
            
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return false;

            if (shape.callbackTarget is not T callbackTarget) 
                return false;
            
            component = callbackTarget;
            return true;
        }
#endregion
        
        public static Component TryGetCallbackComponent(this PhysicsShape shape, bool checkShapeValidation = false)
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return null;

            if (shape.callbackTarget is Component component)
                return component;

            return null;
        }
        
            
        public static T GetCallbackComponent<T>(this PhysicsShape shape, bool checkShapeValidation = false) where T : Component
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return null;

            if (shape.callbackTarget is Component component)
                return component as T;

            return null;
        }
        
        
        /// <summary>
        /// Attempts to get a <see cref="SFShapeComponent"/> that is set as a callback target to a <see cref="PhysicsShape"/>
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="checkShapeValidation"></param>
        /// <returns></returns>
        public static SFShapeComponent GetCallbackShapeComponent(this PhysicsShape shape, bool checkShapeValidation = false)
        {
            // Optional check for only using SFShapeComponents set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return null;

            if (shape.callbackTarget is SFShapeComponent shapeComponent)
                return shapeComponent;

            return null;
        }
        
         public static bool TryGetCallbackAsType<TType>(
             this PhysicsBody body, 
             out TType targetCallback, 
             bool checkShapeValidation = false)
         {
             /* Since we want to support both nullable reference and value types for our "out" TType targetCallback,
                we set the starting value as default. If TType is a value type it gets sets like to the default normal type.
                If it is a reference it is set as null.*/

             targetCallback = default;
             
             // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !body.isValid)
                return false;


            if (body.callbackTarget is not TType target) 
                return false;
            
            targetCallback = target;
            return true;
         }
        
        public static Component TryGetCallbackComponent(this PhysicsBody body, bool checkShapeValidation = false)
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !body.isValid)
                return null;

            if (body.callbackTarget is Component component)
                return component;

            return null;
        }
        
        /// <summary>
        /// Attempts to get a <see cref="SFShapeComponent"/> that is set as a callback target to a <see cref="PhysicsBody"/>
        /// </summary>
        /// <param name="body"></param>
        /// <param name="checkShapeValidation"></param>
        /// <returns></returns>
        public static SFShapeComponent TryGetCallbackShapeComponent(this PhysicsBody body, bool checkShapeValidation = false)
        {
            // Optional check for only using SFShapeComponents set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !body.isValid)
                return null;

            if (body.callbackTarget is SFShapeComponent shapeComponent)
                return shapeComponent;

            return null;
        }


#region PhysicsShape.ContactFilter

        [BurstCompile]
        public static void SetCategoriesBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.categories;
            mask.SetBit(bitIndex);
            filter.categories = mask;
        }
        
        [BurstCompile]
        public static void ResetCategoriesBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.categories;
            mask.ResetBit(bitIndex);
            filter.categories = mask;
        }
        [BurstCompile]
        public static void SetContactsBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.contacts;
            mask.SetBit(bitIndex);
            filter.contacts = mask;
        }
        
        [BurstCompile]
        public static void ResetContactsBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.contacts;
            mask.ResetBit(bitIndex);
            filter.contacts = mask;
        }
        
#endregion

    }
}
