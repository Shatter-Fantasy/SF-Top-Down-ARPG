using System.Collections.Generic;

using UnityEngine;
using ZTDR.PhysicsLowLevel;

namespace SF
{
    using InputModule;
    using Interactables;
    
    public class InteractableSwitch : MonoBehaviour, IInteractable
    {

        public List<ActivableWrapper> Activatables = new List<ActivableWrapper>();

        [SerializeField] private bool _oneTimeUse = false;
        private bool _wasUsed = false;

        [field: SerializeField] public InteractableMode InteractableMode { get; set; }


        public void Interact()
        {
            if(InteractableMode != InteractableMode.Input && !SFInputManager.Controls.Player.Interact.WasPressedThisFrame())
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
    }
}