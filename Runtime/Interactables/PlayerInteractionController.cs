using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    using InputModule;
    using PhysicsLowLevel;
    
    public class PlayerInteractionController : InteractionController, ITriggerShapeCallback
    {
        private PlayerControllerBody2D _controller;
        
        private void Awake()
        {
            TryGetComponent(out _controller);
            if(_hitShape == null)
                TryGetComponent(out _hitShape);
        }
        
              
        private void OnEnable()
        {
            SFInputManager.Controls.Player.Enable();
            SFInputManager.Controls.Player.Interact.performed += OnInteractPerformed;
            SFInputManager.Controls.Player.Interact.started += OnInteractPerformed;

            if (_hitShape != null)
            {
                _hitShape.AddTriggerCallbackTarget(this);
                _hitShape.Shape.contactFilter.contacts.SetBit(SFPhysicsManager.InteractableLayer);
                _hitShape.Body.enabled  = false;
            }
        }

        private void OnDisable()
        {
            if(SFInputManager.Controls == null) return;
            
            SFInputManager.Controls.Player.Interact.performed -= OnInteractPerformed;
            SFInputManager.Controls.Player.Interact.started   -= OnInteractPerformed;
            if(_hitShape != null)
                _hitShape.RemoveTriggerCallbackTarget(this);
        }
        
        protected void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if(!_hitShape.Shape.isValid) return;
            _hitShape.Body.position = transform.position;
            _hitShape.Body.enabled  = true;
            _hitShapes              = new NativeArray<PhysicsShape>(5, Allocator.Temp);
            using var result = _hitShape.PhysicsWorld.OverlapShape(_hitShape.Shape, _interactableFilter);
            _hitShape.Body.enabled  = false;
            
            if (result.Length < 1)
                return;

            // This is a painful looking thing, but it is actually decent performance, so oh well.
            for (int i = 0; i < result.Length; i++)
            {
                // Grab the body data.
                var objectData = result[i].shape.body.userData.objectValue;
                
                if (objectData is GameObject hitObject 
                    && hitObject.TryGetComponent(out IInteractable interactable)
                    && interactable.InteractableMode == InteractableMode.Input)
                {
                    if(interactable is IInteractable<PlayerControllerBody2D> interactableController
                            && _controller is not null)
                        interactableController.Interact(_controller);
                    else
                        interactable.Interact();
                }
            }
        }
  
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            // Grab the body data.
            var objectData = beginEvent.triggerShape.body.userData.objectValue;
            
            if (objectData is GameObject hitObject
                && hitObject.TryGetComponent(out IInteractable interactable)
                && interactable.InteractableMode == InteractableMode.Collision)
            {
                if(interactable is IInteractable<PlayerControllerBody2D> interactableController
                   && _controller is not null)
                    interactableController.Interact(_controller);
                else
                    interactable.Interact();
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // noop - No Operation
        }
    }
}
