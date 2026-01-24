using System;
using SF.Interactables;
using SF.Managers;
using SF.PhysicsLowLevel;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using ZTDR.PhysicsLowLevel;

namespace SF.ItemModule
{
    using Interactables;
    public class PickupItem : MonoBehaviour, 
        IInteractable<PlayerControllerBody2D>, 
        ITriggerShapeCallback
    {
        [field: SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public ItemData Item;

        private void Start()
        {
            if (TryGetComponent(out SFShapeComponent component))
                component.AddTriggerCallbackTarget(this);
        }

        public void Interact()
        {
            
        }

        public void Interact(PlayerControllerBody2D controller)
        {
            if(controller == null || Item == null)
                return;
           
            // Make sure we added an instantiated inventory to the player first.
            if(controller.TryGetComponent(out PlayerInventory playerInventory))
            {
                PickUpItem(playerInventory);
            }
        }

        private void PickUpItem(PlayerInventory playerInventory)
        {         
            playerInventory.AddItem(Item.ID);
            Destroy(gameObject);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;
           
            if (beginEvent.visitorShape.callbackTarget is not PlayerControllerBody2D body2D)
                return;
                    
            if(body2D.CollisionInfo.CollisionActivated)
            {
                Interact(body2D);
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
        {
            // noo - No Operation
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            OnTriggerBegin2D(beginEvent);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        { 
            // noo - No Operation
        }
    }
}
