using UnityEngine;

namespace SF.LootModule
{
    using DataModule;
    
    [CreateAssetMenu(fileName = "LootTableDatabase", menuName = "SF/Item System/Loot Data/Loot Table Database")]
    public class LootTableDatabase : SFAssetDatabase<LootTableData>
    {
    }
}
