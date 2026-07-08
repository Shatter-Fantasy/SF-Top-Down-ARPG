using System;
using UnityEngine;

namespace SF.AbilityModule
{
    /// <summary>
    /// Not fully implemented yet.
    /// This will allow for abilities to have cooldowns for a variety of 
    /// cases such as ability reactivation, ability delay for going from one ability to another, and ability activation delay when leaving certain character states. Think of a delay on ability activation after being damaged of stunned.
    /// </summary>
    [Serializable]
    public class AbilityCooldown
    {
        /// <summary>
        /// The timer that controls the cooldown state.
        /// </summary>
        public Timer CoolDownTimer;

        /// <summary>
        /// Is the ability currently on cooldown.
        /// </summary>
        public bool IsOnCooldown = false;
        
        private Action _onCooldownCompleted;

        public AbilityCooldown(float cooldownTime = 2.5f, Action onCooldownCompleted = null)
        {
            _onCooldownCompleted = onCooldownCompleted;
            CoolDownTimer = new Timer(cooldownTime, _onCooldownCompleted);
        }
        
        public void Start(bool bypassCooldown = false)
        {
            if (IsOnCooldown && !bypassCooldown)
                return;
            
            StartCooldownAsync();
        }
        
        private async void StartCooldownAsync()
        {
            try
            {
                // First set it to false
                IsOnCooldown = true;
                // The CooldownTimer awaitable will return true when the timer finishes and after the onCompleted is run./
                IsOnCooldown = !await CoolDownTimer.StartTimerAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
