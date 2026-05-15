using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SF.SpawnModule
{
    using Characters.Data;
    using SF.DataModule;
    using RoomModule;
    using StatModule;
    
    [System.Serializable]
    public struct SpawnSet
    {
        public int SpawnCharacterID;
        public Vector2 SpawnPosition;
        public GameObject SpawnedCharacter;
        public CharacterHealth SpawnedHealth;
    }
    
    /// <summary>
    /// Controls the spawn characters to keep spawned or despawn based on the current room that the player is in. 
    /// </summary>
    public class RoomCharacterSpawner : MonoBehaviour
    {

        [Header("Character Data")]
        public CharacterDatabase CharacterDB;
        public SpawnSet[] SpawnSets;
        
        private bool _alreadySpawned;
        private int _roomCharacterCount = 0; 

        private RoomController _roomController;
        
        private void Awake()
        {
            TryGetComponent(out _roomController);
            if (CharacterDB == null)
                CharacterDB = DatabaseRegistry.GetDatabase<CharacterDatabase>();
        }
        
        private void OnEnable()
        {
            if (_roomController == null)
                return;
            
            _roomController.OnRoomEnteredHandler += SpawnCharacters;
            _roomController.OnRoomExitHandler    += DespawnCharacters;
        }
        
        private void OnDisable()
        {
            if (_roomController == null)
                return;
            
            _roomController.OnRoomEnteredHandler -= SpawnCharacters;
            _roomController.OnRoomExitHandler    -= DespawnCharacters;
        }
        

        private void SpawnCharacters()
        {
            // Don't spawn the characters when they are already loaded in memory.
            if (_alreadySpawned)
            {
                RespawnCharacters();
                return;
            }

            if (SpawnSets.Length < 1)
                return;

            _roomCharacterCount = SpawnSets.Length;
            
            if(CharacterDB == null)
                return;
            
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                var spawnedCharacterData = CharacterDB.GetDataByID(SpawnSets[i].SpawnCharacterID);
                SpawnSets[i].SpawnedCharacter = Instantiate(spawnedCharacterData.Prefab, SpawnSets[i].SpawnPosition, Quaternion.identity);
                

                if(!SpawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterStats stats))
                {
                    stats = SpawnSets[i].SpawnedCharacter.AddComponent<CharacterStats>();
                }

                stats.CharacterStatList                          =  spawnedCharacterData.Stats;
                SpawnSets[i].SpawnedHealth                       =  stats.CharacterHealth;
                SpawnSets[i].SpawnedHealth.CharacterDeathHandler += OnCharacterDeath;
                SpawnSets[i].SpawnedHealth.Respawn();
                

                if(!SpawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterData characterData))
                {
                    characterData = SpawnSets[i].SpawnedCharacter.AddComponent<CharacterData>();
                }
                    
                if(characterData is CombatantData cData)
                {

                    cData.SetData(spawnedCharacterData);
                }
                else
                {
                    characterData.SetData(spawnedCharacterData);
                }

            } // End of for loop.

            _alreadySpawned = true;
        }

        private void OnCharacterDeath(CharacterHealth obj)
        {
            _roomCharacterCount--;

            // If all enemies are killed invoke OnRoomClearedEvents
            if (_roomCharacterCount <= 0)
            {
                _roomController.OnRoomCleared();
            }
        }

        private void DespawnCharacters()
        {
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                if(SpawnSets[i].SpawnedCharacter == null)
                    continue;
                
                SpawnSets[i].SpawnedHealth?.Despawn();
            }
        }

        /// <summary>
        /// Called if the characters were already loaded, but than respawned.
        /// Happens when first loading a room and it's characters than exiting the room despawning them.
        /// </summary>
        private void RespawnCharacters()
        {
            _roomCharacterCount = SpawnSets.Length;
            
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                if(SpawnSets[i].SpawnedCharacter == null)
                    continue;
                
                SpawnSets[i].SpawnedHealth.Respawn();
                SpawnSets[i].SpawnedCharacter.transform.position = SpawnSets[i].SpawnPosition;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RoomCharacterSpawner))]
    public class RoomCharacterSpawnerEditor : Editor
    {
        private Vector3 _lastFramePosition;
        private RoomCharacterSpawner _target;
        private void Awake()
        {
            _target = target as RoomCharacterSpawner;
            
            if(_target != null)
                _lastFramePosition = _target.transform.position;
        }

        public void OnSceneGUI()
        {
            RoomCharacterSpawner t = target as RoomCharacterSpawner;
            
            if (t is null || t.SpawnSets?.Length < 1)
                return;

            if (_lastFramePosition != _target.transform.position)
            {
                Vector2 deltaPosition = _target.transform.position - _lastFramePosition;
                for (int i = 0; i < t.SpawnSets?.Length; i++)
                {
                    t.SpawnSets[i].SpawnPosition += deltaPosition;
                }

                _lastFramePosition = _target.transform.position;
            }
            
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;

            for (int i = 0; i < t.SpawnSets?.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(t.SpawnSets[i].SpawnPosition, Quaternion.identity);

                if (t.CharacterDB != null &&
                    t.CharacterDB.GetDataByID(t.SpawnSets[i].SpawnCharacterID, out CharacterDTO characterDTO))
                {
                    Handles.Label(newTargetPosition, $"{characterDTO.name}");
                    if (characterDTO.Prefab?.GetComponent<SpriteRenderer>() != null)
                    {
                        
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Change Look At Target Position");
                    t.SpawnSets[i].SpawnPosition = newTargetPosition;
                }
            }
        }
    }
#endif
}
