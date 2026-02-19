using SF.PhysicsLowLevel;
using Unity.Burst;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace ZTDR.StateMachine
{
    using SF.StateMachine.Core;
    [BurstCompile]
    public class MovementRandomAIState : StateCore, IContactShapeCallback
    {
        [SerializeField] private Vector2 _randomMovementRange = new Vector2(1.5f,1.5f);
        [SerializeField] private float _stopDistance = .5f;
        
        private PhysicsBody _physicsBody;
        private Vector2 _targetPosition;
        private Vector2 _lastPositionDelta;

        protected override void OnStateEnter()
        {
            _controllerBody2D.ShapeComponent.AddContactCallbackTarget(this);
            _controllerBody2D.ShapeComponent.Shape.contactEvents = true;
            
            _physicsBody                                         = _controllerBody2D.ShapeComponent.Body;
            
            if (!_physicsBody.isValid)
                return;
            
            CalculateRandomRange(ref _randomMovementRange,out Vector2 randomRange);
            _targetPosition = _physicsBody.position + randomRange;
        }

        protected override void OnStateExit()
        {
            base.OnStateExit();
            _controllerBody2D.ShapeComponent.RemoveContactCallbackTarget(this);
        }

        protected override void OnUpdateState()
        {
            if(!_physicsBody.isValid)
                return;
            _lastPositionDelta = _physicsBody.position - _targetPosition;
            Move(_stopDistance,ref _randomMovementRange, ref _physicsBody, ref _targetPosition);
        }

        [BurstCompile]
        public static void Move(float stopDistance, ref Vector2 randomMovementRange, 
            ref PhysicsBody physicsBody, ref Vector2 targetPosition)
        {
            // Get distance to target position.
            float distance = Vector2.Distance(physicsBody.position, targetPosition);
            
            if (distance > stopDistance)
            {
                //physicsBody.position = Vector2.Lerp(physicsBody.position, targetPosition, Time.deltaTime);
                Vector2 deltaPosition = targetPosition - physicsBody.position;
                physicsBody.linearVelocity = deltaPosition;
            }
            else
            {
                CalculateRandomRange(ref randomMovementRange,out Vector2 randomRange);
                targetPosition = physicsBody.position + randomRange;
            }
        }

        [BurstCompile]
        public static void CalculateRandomRange(ref Vector2 randomMovementRange, out Vector2 randomRange)
        {
            randomRange = new Vector2(
                    Random.Range(-randomMovementRange.x, randomMovementRange.x),
                    Random.Range(-randomMovementRange.y, randomMovementRange.y)
                );
        }

        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            _targetPosition = _physicsBody.position + _lastPositionDelta;
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            //no-op
        }
    }
}
