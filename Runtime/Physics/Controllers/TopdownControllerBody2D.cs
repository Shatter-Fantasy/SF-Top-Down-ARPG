using UnityEngine;

namespace SF.PhysicsLowLevel
{
    using Characters;
    
    /// <summary>
    /// Base class for all topdown style physic controllers.
    /// </summary>
    public class TopdownControllerBody2D : PhysicController2D
    {
        [Header("Collision Properties")]
        public SFShapeComponent ShapeComponent;

        [SerializeField] public BodyCollisionInfo CollisionInfo = new BodyCollisionInfo();

        [Header("Character States")]
        public CharacterState CharacterState;
        
        [Header("Movement Booleans")] 
        public bool IsRunning;
        public bool IsJumping;
        public bool IsFalling;
        public bool IsGliding;
        public bool IsClimbing;

        protected override void OnAwake()
        {
            DefaultPhysics.GravityScale = 0;
            CollisionInfo = new BodyCollisionInfo()
            {
                ControllerBody2D = this
            };

            if (!TryGetComponent(out ShapeComponent))
            {
                ShapeComponent = gameObject.AddComponent<SFRectangleShape>();
            }
        }

        protected override void FixedUpdate()
        {
            if (_direction.x != 0 || _direction.y != 0)
            {
                DirectionLastFrame = _direction;
            }

            if (!ShapeComponent.Shape.isValid)
                return;
            
            OnPreFixedUpdate();
            
            // TODO: Move this to PhysicsEvent.PostSimulate.
            CollisionInfo.CheckCollisions();
            
            CalculateHorizontal();
            CalculateVertical();
            
            Move();
        }

        protected override void Move()
        {
            if (ShapeComponent == null
                || !ShapeComponent.Shape.isValid)
                return;
            
            if(IsFrozen
               || CharacterState.CharacterStatus == CharacterStatus.Dead)
                _calculatedVelocity = Vector2.zero;
            
            if (_externalVelocity != Vector2.zero)
            {
                _calculatedVelocity = _externalVelocity;
                _externalVelocity = Vector2.zero;
            }

            ShapeComponent.Body.linearVelocity = _calculatedVelocity;
        }

        protected override void CalculateHorizontal()
        {
            // TODO: Check if climbing up a vie or something.

            if (Direction.x != 0)
            {
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                _calculatedVelocity.x = Mathf.MoveTowards(
                    _calculatedVelocity.x, 
                    ReferenceSpeed * Direction.x,
                    CurrentPhysics.GroundAcceleration);
            }
            else // No Input Direction
            {
                _calculatedVelocity.x = ShapeComponent.Body.linearVelocity.x;
                    //Mathf.MoveTowards(_calculatedVelocity.x, 0, CurrentPhysics.GroundDeacceleration);
            }
            
            if (CollisionInfo.IsCollidingLeft && Direction.x < 0)
            {
                _calculatedVelocity.x = 0;
            }
            
            if (CollisionInfo.IsCollidingRight && Direction.x > 0)
            {
                _calculatedVelocity.x = 0;
            }
        }

        protected override void CalculateVertical()
        {
            // TODO: Climbing check
            if (Direction.y != 0)
            {
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                _calculatedVelocity.y = Mathf.MoveTowards(
                    _calculatedVelocity.y, 
                    ReferenceSpeed * Direction.y,
                    CurrentPhysics.GroundAcceleration);
            }
            else // No Input Direction
            {
                // By using ShapeComponent.Body.linearVelocity we get the velocity with linear damping applied for a slowdown.
                _calculatedVelocity.y = ShapeComponent.Body.linearVelocity.y;
            }
            
            if (CollisionInfo.IsCollidingBelow && Direction.y < 0)
            {
                _calculatedVelocity.y = 0;
            }
            
            if (CollisionInfo.IsCollidingAbove && Direction.y > 0)
            {
                _calculatedVelocity.y = 0;
            }
        }

        protected override void CalculateMovementState()
        {
           // no-op - no operation
        }
        
        public void ResizeCollider(Vector2 newSize)
        {
            
        }
        
        public void ResetColliderSize()
        {
            
        }
    }
}
