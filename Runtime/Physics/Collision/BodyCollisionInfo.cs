using System.Linq;
using SF.PhysicsLowLevel;
using SF.PhysicsLowLevel.Utilities;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace ZTDR.PhysicsLowLevel
{
    [System.Serializable]
    public class BodyCollisionInfo : CollisionInfoBase
    {
        public TopdownControllerBody2D ControllerBody2D;
        
        private ContactExtensions.ContactNormalFilterFunction _filterFunction;
        private NativeArray<PhysicsShape.Contact> _contacts;


        public override void CheckCollisions()
        {
            WasCollidingLeft  = IsCollidingLeft;
            WasCollidingRight = IsCollidingRight;
            WasCollidingAbove = IsCollidingAbove;
            WasCollidingBelow = IsGrounded;
			
            WasGroundedLastFrame = IsGrounded;
            
            using (_contacts = ControllerBody2D.ShapeComponent.Shape.GetContacts())
            { 
                BottomCollisionChecks();
                TopCollisionChecks();
                SideCollisionChecks();
                CheckOnCollisionActions();
            }
            
            _contacts.Dispose();
        }

        public override void SideCollisionChecks()
        {
            if (_contacts.Length == 0)
            {
                IsCollidingRight = false;
                IsCollidingLeft  = false;
                return;
            }
            
            // Left Collision Check
            var filteredContacts = _contacts.Filter(
                ContactFiltering.NormalXFilter, 
                ControllerBody2D.ShapeComponent.Shape, 
                CollisionContactThreshold,
                FilterMathOperator.GreaterThan);
            
            IsCollidingLeft = (filteredContacts.ToList().Count > 0);
            
            // Right Collision Check
            filteredContacts = _contacts.Filter(
                ContactFiltering.NormalXFilter, 
                ControllerBody2D.ShapeComponent.Shape, 
                -CollisionContactThreshold,
                FilterMathOperator.LessThan);
            
            IsCollidingRight = (filteredContacts.ToList().Count > 0);
        }

    }
}
