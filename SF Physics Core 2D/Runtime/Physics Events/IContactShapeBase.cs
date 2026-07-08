using Unity.Burst;
using Unity.U2D.Physics;
using UnityEngine;

namespace SF.U2D.Physics
{
    [System.Flags]
    public enum ContactDirection : short
    {
        Nothing = 0,
        Left = 1,
        Right = 2,
        Sides = 3,
        Up = 4,
        Down = 8,
        Any = Down ^ Up ^ Left ^ Right
    }
    
    [BurstCompile]
    public struct ContactBeginResult
    {
        public PhysicsEvents.ContactBeginEvent BeginEvent;
        public SFShapeComponent CallingShapeComponent;

        /// <summary>
        /// The <see cref="PhysicsShape"/> on the <see cref="CallingShapeComponent"/>
        /// </summary>
        public PhysicsShape CallingShape;
        /// <summary>
        /// The incoming <see cref="PhysicsShape"/>
        /// </summary>
        public PhysicsShape IncomingShape;
        
        /// <summary>
        /// Returns if both the <see cref="CallingShape"/> and the <see cref="IncomingShape"/> are valid with the <see cref="ContactID"/>
        /// also being a still valued conctact
        /// </summary>
        public readonly bool IsValid => BeginEvent.shapeA.isValid && BeginEvent.shapeB.isValid && ContactID.isValid;

        public readonly Vector2 Normal;
        public readonly ContactDirection ContactDirection;

        public readonly PhysicsShape.ContactManifold Manifold;
        public readonly PhysicsShape.ContactId ContactID;
        
        public ContactBeginResult(in PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            CallingShapeComponent = callingShapeComponent;
            if (CallingShapeComponent == null)
            {
                BeginEvent = default;
                ContactID = default;
                CallingShape = default;
                IncomingShape = default;
                Manifold = default;
                Normal = Vector2.zero;
                ContactDirection = ContactDirection.Nothing;
                return;
            }
            BeginEvent = beginEvent;
            ContactID = BeginEvent.contactId;
            Manifold = BeginEvent.contactId.contact.manifold;
            
            if (CallingShapeComponent.CompareShapeEntityID(BeginEvent.shapeA))
            {
                CallingShape = BeginEvent.shapeA;
                IncomingShape = BeginEvent.shapeB;
                Normal = Manifold.normal.normalized;
            }
            else
            {
                CallingShape = BeginEvent.shapeB;
                IncomingShape = BeginEvent.shapeA;
                // We have to flip the normal because the contact manifold calculated from the other direction.
                Normal = Manifold.normal.normalized * -1;
            }
            ContactDirection = GetContactDirectionStatic(Normal);
        }

        public ContactDirection GetContactDirection() => GetContactDirectionStatic(Normal);
        
        [BurstCompile]
        private static ContactDirection GetContactDirectionStatic(in Vector2 normal)
        {
            // TODO: Add diagnoal check support.
            if(normal == Vector2.left)
                return Physics.ContactDirection.Left;
            if(normal == Vector2.right)
                return Physics.ContactDirection.Right;
            if(normal == Vector2.down)
                return Physics.ContactDirection.Down;
            if(normal == Vector2.up)
                return Physics.ContactDirection.Up;

            return ContactDirection.Nothing;
        }

        public bool WasContactFromDirection(ContactDirection direction)
        {
            return (Normal == Vector2.left && (direction & ContactDirection.Left) != 0)
                   || (Normal == Vector2.right && (direction & ContactDirection.Right) != 0)
                   || (Normal == Vector2.down && (direction & ContactDirection.Down) != 0)
                   || (Normal == Vector2.up && (direction & ContactDirection.Up) != 0);
        }
    }
    public interface IContactShapeCallbackBase
    {
    
    }
    
    public interface IContactShapeCallback : IContactShapeCallbackBase
    {
        void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent);

        void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent);
    }
}
