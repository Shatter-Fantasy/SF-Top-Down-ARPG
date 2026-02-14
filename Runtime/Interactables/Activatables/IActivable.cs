using System;
using UnityEngine;

namespace SF.Interactables
{
    public interface IActivable
    {
        public bool Activated{ get; set; }
    }

    
    public interface IActivableEventHandler : IActivable
    {
        public event Action OnActivationHandler;
        public event Action OnDeactivationHandler;
        /// <summary>
        /// Called when the activable is reset.
        /// <example>
        /// Example usage is if you have levers that reset when you exit and renter a room where they were moved to a different position,
        /// but needs moved back to the original spot on entering.
        /// </example>
        /// </summary>
        public event Action OnResetActivableHandler;
    }

    public abstract class ActivableWrapper : MonoBehaviour, IActivable
    {
        [field: SerializeField]
        private bool _activated;
        public bool Activated
        {
            get => _activated;
            set
            {
                // If we are activating while currently not already active.
                if(value && !_activated)
                {
                    OnActivation();
                }
                // If we are deactivating it while it is currently activated
                else if(!value && _activated)
                {
                    OnDeactivate();
                }
                _activated = value;
            }
        }

        protected virtual void OnActivation()
        {

        }
        protected virtual void OnDeactivate()
        {

        }
    }
}