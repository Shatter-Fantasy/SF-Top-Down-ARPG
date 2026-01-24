using SF.SpawnModule;
using UnityEngine;
using ZTDR.PhysicsLowLevel;

namespace SF.DataManagement
{
    using Interactables;
    using RoomModule;
    using StatModule;
    
    public class SaveStation : CheckPoint, IInteractable<PlayerControllerBody2D>
    {
        /// <summary>
        /// The room id that the save room is in.
        /// </summary>
        public int RoomID;

        [field:SerializeField] public InteractableMode InteractableMode { get; set; }
        public virtual void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }

        public virtual void Interact(PlayerControllerBody2D controller)
        {
            if(controller.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.FullHeal();
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerHealth = health;
            }
            
            if(controller.TryGetComponent<PlayerStats>(out PlayerStats stats))
            {
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerStats = stats;
            }
            
            MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.SavedRoomID = RoomSystem.CurrentRoom.RoomID;
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }
    }
}
