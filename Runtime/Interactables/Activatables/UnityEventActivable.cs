using UnityEngine;
using UnityEngine.Events;

namespace SF.Interactables
{
    public class UnityEventActivable : ActivableWrapper
    {
        [SerializeField] private UnityEvent _onActivatedEvent;
        [SerializeField] private UnityEvent _onDeactivatedEvent;

        protected override void OnActivation()
        {
            _onActivatedEvent?.Invoke();
        }

        protected override void OnDeactivate()
        {
            _onDeactivatedEvent?.Invoke();
        }
    }
}
