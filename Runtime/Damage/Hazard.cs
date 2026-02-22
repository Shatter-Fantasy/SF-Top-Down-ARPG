using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.DamageModule
{
    using PhysicsLowLevel;
    
    [System.Flags]
    public enum Direction : short
    {
        Any = 0,
        Left = 1,
        Right = 2,
        Sides = 3,
        Up = 4,
        Down = 8,
    }

    public class Hazard : MonoBehaviour, 
        IDamage, 
        ITriggerShapeCallback,
        IContactShapeCallback
        
    {
        private Vector2 _collisionNormal;
        public Direction DamageDirection;
        public int DamageAmount = 1;
        [SerializeField] private Vector2 _knockBackForce;
        
        
        private void Start()
        {
            if (TryGetComponent(out SFShapeComponent component))
            {
                component.AddTriggerCallbackTarget(this);
                component.AddContactCallbackTarget(this);
            }
        }
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            // If the target is not a component we can back out.
            if(!beginEvent.TryGetCallbackComponentOnVisitor(out Component callbackTarget))
                return;
            
            if(!callbackTarget.TryGetComponent(out IDamagable damagable))
                return;
            
            damagable.TakeDamage(DamageAmount,_knockBackForce);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // noop - No Operation.
        }
        
        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            // Alright this was broken by me. I should be grabbing the visiting physics shape IDamagable not the IDamagable for the attached component.
            if (!beginEvent.TryGetCallbackComponentOnVisitor(out IDamagable damagable))
                return;
            
            Debug.Log(damagable);
            damagable.TakeDamage(DamageAmount,_knockBackForce);
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            
        }
    }
}
