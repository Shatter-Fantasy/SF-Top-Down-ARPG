using UnityEngine;
using Unity.U2D.Physics;

namespace SF.ItemModule
{
    using Interactables;
    using U2D.Physics;
    using Managers;
    
    public class PickupItem : MonoBehaviour, 
        IInteractable<PlayerControllerBody2D>, 
        ITriggerShapeCallback
    {
        [field: SerializeField] public InteractableMode InteractableMode { get; set; }
        
        [SerializeReference] // If you don't put the new ItemData() by default the SerializeReference will render a blank inspector visual element.
        public ItemData Item = new ItemData();

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
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;
            
            if(!beginEvent.TryGetCallbackComponentOnVisitor(out SFShapeComponent shapeComponent))
                return;

            if (!shapeComponent.TryGetComponent(out PlayerControllerBody2D body2D))
                return;
            
            if(body2D.CollisionInfo.CollisionActivated)
            {
                Interact(body2D);
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        { 
            // noo - No Operation
        }
    }
}
