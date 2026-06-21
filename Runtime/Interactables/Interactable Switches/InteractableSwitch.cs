using System.Collections.Generic;
using Unity.U2D.Physics;
using UnityEngine;

namespace SF.Interactables
{
    using InputModule;
    using U2D.Physics;
    
    public class InteractableSwitch : MonoBehaviour, IInteractable, ITriggerShapeCallback
    {
        public List<ActivableWrapper> Activatables = new List<ActivableWrapper>();

        [SerializeField] private bool _oneTimeUse = false;
        protected bool _wasUsed = false;

        [field: SerializeField] public InteractableMode InteractableMode { get; set; }
        protected SFShapeComponent _hitboxInteractable;

        protected void Awake()
        {
            _hitboxInteractable ??= GetComponent<SFShapeComponent>();
        }

        protected void OnEnable()
        {
            if(_hitboxInteractable != null && (InteractableMode & InteractableMode.TriggerBegin) > 0)
                _hitboxInteractable.AddTriggerCallbackTarget(this);
        }
        
        protected void OnDisable()
        {
            if(_hitboxInteractable != null)
                _hitboxInteractable.RemoveTriggerCallbackTarget(this);
        }

        public virtual void Interact()
        {
            if(InteractableMode == InteractableMode.Input 
               && !SFInputManager.Controls.Player.Interact.WasPressedThisFrame())
                return;

            if(_oneTimeUse && _wasUsed)
                return;

            if(_oneTimeUse)
                _wasUsed = true;

            for(int i = 0; i < Activatables.Count; i++)
            {
                if(Activatables[i] == null)
                    continue;

                Activatables[i].Activated = !Activatables[i].Activated;
            }
        }

        public void Interact(PlayerControllerBody2D controller)
        {
            Interact();
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            Interact();
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
        }
    }
}