using UnityEngine;
using UnityEngine.Events;

namespace SF.Interactables
{
    public class InteractableSwitchUnityEvent : InteractableSwitch
    {
        [Header("Optional Unity Events")]
        [SerializeField] private UnityEvent _onInteractedEvent;
        
        public override void Interact()
        {
            base.Interact();
            _onInteractedEvent?.Invoke();
        }
    }
}
