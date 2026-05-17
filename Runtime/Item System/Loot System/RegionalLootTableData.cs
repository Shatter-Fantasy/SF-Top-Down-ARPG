using UnityEngine;

namespace SF.LootModule
{
	using SF.DataModule;
	
	[CreateAssetMenu(fileName = "Region Table Data", menuName = "SF/Item System/Loot Data/Region Table Data")]
	public class RegionalLootTableData : DTOAssetBase
	{
		public string Region;
	}
}
