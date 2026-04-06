using System;
using UnityEngine;

namespace SF.SpawnModule
{
    public class PlayerHealth : CharacterHealth
    {
        /// <summary>
        /// Allows objects to listen for a change in health and to see the new current health and the max health.
        /// </summary>
        public static Action<int,int> PlayerHealthChangedHandler;

        public override void TakeDamage(int damage, Vector2 knockback = new Vector2())
        {
            base.TakeDamage(damage, knockback);
            PlayerHealthChangedHandler?.Invoke(CurrentHealth,MaxHealth);
        }

        protected override void Kill(Vector2 knockback = new Vector2())
        {
            base.Kill();
            
            SpawnSystem.RespawnPlayer();
        }

        public override void Respawn()
        {
            // TODO: Remove CheckPointManager and use the SpawnSystem.
            if(CheckPointManager.Instance == null)
                return;
            
            // TODO: Remove CheckPointManager and use the SpawnSystem.
            if(CheckPointManager.Instance.CurrentCheckPoint != null)
                transform.position = CheckPointManager.Instance.CurrentCheckPoint.transform.position;

            base.Respawn();
        }

        public override void Despawn()
        {
            // This empty ovveride prevents the base health script from deactivating the player.
            // Only needed for a bit before the next update to the player spawn system is done.
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            SpawnSystem.PlayerRespawnHandler += Respawn;
        }

        protected override void OnDisable()
        {
            // Might need to move this to the OnDestroy if we disable the player during respawning.
            SpawnSystem.PlayerRespawnHandler -= Respawn;
        }
    }
}
