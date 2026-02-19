namespace SF.DataManagement
{
    using PhysicsLowLevel;
    using SpawnModule;
    using RoomModule;
    using StatModule;
    
    public class MetroidvaniaSaveStation : SaveStation
    {
        
        
        public override void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }

        public override void Interact(PlayerControllerBody2D controller)
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
