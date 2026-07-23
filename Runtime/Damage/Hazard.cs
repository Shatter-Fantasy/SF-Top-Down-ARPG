using UnityEngine;
using Unity.U2D.Physics;

namespace SF.DamageModule
{
    using SF.U2D.Physics;
    
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

        [SerializeField] private PhysicsMask _targetPhysicsMask = new (SFPhysicsManager.PlayerLayer);
        
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
            PhysicsShape damageableShape = beginEvent.GetShapeWithCategoryBit(SFPhysicsManager.PlayerLayer);

            // (PhysicsShape)default should be false when using SIValid.
            if (!damageableShape.isValid)
                return;

            damageableShape.TryGetCallbackComponent(out IDamagable damagable);
            damagable.TakeDamage(DamageAmount,_knockBackForce);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // noop - No Operation.
        }
        
        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if (!beginEvent.TryGetShapeWithCategoryBit(SFPhysicsManager.PlayerLayer, out PhysicsShape damageableShape))
                return;

            if(damageableShape.TryGetCallbackComponent(out IDamagable damagable))
                damagable.TakeDamage(DamageAmount,_knockBackForce);
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            
        }
    }
}