using UnityEngine;
using Unity.U2D.Physics;

namespace SF.DialogueModule
{
    using Interactables;
    using U2D.Physics;
    
    public class DialogueInteractable : MonoBehaviour, 
        IInteractable<PlayerControllerBody2D>,
        ITriggerShapeCallback
    {
        [SerializeField] private DialogueConversation _dialogueConversation;

        [field: Space()] [field: SerializeField]
        public InteractableMode InteractableMode { get; set; } = InteractableMode.Input;

        [SerializeField] private SFShapeComponent _shapeComponent;
        
        private void Awake()
        {
            _shapeComponent?.AddTriggerCallbackTarget(this);
        }

        public void Interact() {  }
        public void Interact(PlayerControllerBody2D controller)
        {
            if(_dialogueConversation != null)
                DialogueManager.TriggerConversation(_dialogueConversation,this);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if(_dialogueConversation != null && InteractableMode == InteractableMode.Collision)
                DialogueManager.TriggerConversation(_dialogueConversation,this);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            DialogueManager.StopConversation();
        }
    }
}