using SF.PhysicsLowLevel;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace ZTDR.StateMachine
{
    using SF.StateMachine.Core;
    [BurstCompile]
    public class MovementRandomAIState : StateCore
    {
        [SerializeField] private Vector2 _randomMovementRange = new Vector2(1.5f,1.5f);
        [SerializeField] private float _stopDistance = .5f;
        [SerializeField] private float _chanceToChangeDirection = 20;

        private bool _updateDirection = true;
        private PhysicsBody _physicsBody;
        public Vector2 Velocity;
        private Vector2 _targetPosition;

        private Vector2 MovementDirection => _targetPosition - _physicsBody.position;
        protected override void OnStateEnter()
        {
            ref var physicsBody    = ref _controllerBody2D.ShapeComponent.Body;
            
            if (!physicsBody.isValid)
                return;
            
            CalculateRandomRange(ref _randomMovementRange, out Velocity);
            _controllerBody2D.SetVelocity(Velocity);

            return;
            
            CalculateRandomRange(ref _randomMovementRange,out Vector2 randomRange);
            _targetPosition = _physicsBody.position + randomRange;
        }

        protected override void OnUpdateState()
        {
            if(!_physicsBody.isValid)
                return;

            CalculateRandomRange(ref _randomMovementRange, out Vector2 velocity);
            _physicsBody.linearVelocity = velocity;
            //Move(_stopDistance,ref _randomMovementRange, _physicsBody, ref _targetPosition);
            
            /*
            var world = _physicsBody.world;
            if (!world.isValid)
                return;
            
            var shape = _controllerBody2D.ShapeComponent.Shape;
            if (!shape.isValid)
                return;
            var translation = _physicsBody.position*/
        }

        [BurstCompile]
        public static void Move(float stopDistance, ref Vector2 randomMovementRange, in PhysicsBody physicsBody, ref Vector2 targetPosition)
        {
            // Get distance to target position.
            float distance = Vector2.Distance(physicsBody.position, targetPosition);
            
            if (distance > stopDistance)
            {
                physicsBody.position = Vector2.Lerp(physicsBody.position, targetPosition, Time.deltaTime);
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

       
    }
}
