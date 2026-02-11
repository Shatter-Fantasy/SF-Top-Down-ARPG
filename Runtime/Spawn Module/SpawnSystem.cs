using System;
using UnityEngine;
using ZTDR.PhysicsLowLevel;

namespace SF.SpawnModule
{
    using RoomModule;
    /// <summary>
    /// The system that controls the logic for spawning the player. 
    /// </summary>
    public class SpawnSystem : MonoBehaviour
    {
        public GameObject Controller;

        public Transform DebugSpawn;
        private void Start()
        {
            if (Controller != null)
                OnInitialPlayerSpawn(Controller, DebugSpawn);
        }

        private void OnDestroy()
        {
            CurrentSpawnPosition = null;
            SpawnedPlayer = null;
            SpawnedPlayerController = null;
        }

        public static Transform CurrentSpawnPosition;
        
        /// <summary>
        /// The spawned root gameobject of the player.
        /// </summary>
        public static GameObject SpawnedPlayer;
        
        /// <summary>
        /// The <see cref="ControllerBody2D"/> of the <see cref="SpawnedPlayer"/>.
        /// </summary>
        public static TopdownControllerBody2D SpawnedPlayerController;
        
        public static event Action<GameObject> InitialPlayerSpawnHandler;
        public static event Action PlayerRespawnHandler;

        /// <summary>
        /// Tell the game to start the initial spawning of the player when loading up a save file.
        /// </summary>
        public static TopdownControllerBody2D OnInitialPlayerSpawn(GameObject playerPrefab, Transform defaultSpawn = null)
        {
            if (playerPrefab == null)
                return null;

            if (defaultSpawn != null)
            {
                SpawnedPlayer = GameObject.Instantiate(playerPrefab,defaultSpawn.position,Quaternion.identity);
            }
            else
            {
                SpawnedPlayer = GameObject.Instantiate(playerPrefab,RoomSystem.CurrentRoom.SpawnedInstance.transform.position,Quaternion.identity);
            }
          
            if (SpawnedPlayer == null)
                return null;
            
            // Instead of the RoomSystem current room we should use the active spawn point.
            //SpawnedPlayer.transform.position = RoomSystem.CurrentRoom.SpawnedInstance.transform.position;
            SpawnedPlayer.TryGetComponent(out SpawnedPlayerController);
            
            InitialPlayerSpawnHandler?.Invoke(SpawnedPlayer);
            
            return SpawnedPlayer.GetComponent<PlayerControllerBody2D>();
        }

        /// <summary>
        /// Respawns the player and invokes the <see cref="PlayerRespawnHandler"/> event.
        /// </summary>
        public static void RespawnPlayer()
        {
            PlayerRespawnHandler?.Invoke();
        }
    }
}
