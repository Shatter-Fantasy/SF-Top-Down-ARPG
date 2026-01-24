using SF.LootModule;
using UnityEngine;

namespace SF.Characters.Data
{
    public class CombatantData : CharacterData
    {
        public LootTableData EnemyLootTable;
        public RegionalLootTableData RegionalLootTable;
        
        
#if UNITY_EDITOR
        [SerializeField] private bool _debugSpawn;
        [SerializeField] private CharacterDTO _debugCharacterDTO;
        
        private void Start()
        {
            if (_debugCharacterDTO == null)
                return;
            
            SetData(_debugCharacterDTO);
        }
#endif
        
        public override void SetData(CharacterDTO dto)
        {
            base.SetData(dto);
            EnemyLootTable = dto.EnemyLootTable;
            RegionalLootTable = dto.RegionalLootTable;
        }
    }
}