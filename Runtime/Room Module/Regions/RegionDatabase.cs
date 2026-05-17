using UnityEngine;

namespace SF.RoomModule.RegionModule
{
    using DataModule;
    //using static Managers.GameDefaultExecutionOrders;
    
    //[DefaultExecutionOrder(DatabaseExecutionOrder)]
    [CreateAssetMenu(fileName = "RegionDatabase", menuName = "SF/Regions/Region Database")]
    public class RegionDatabase : SFAssetDatabase<RegionDataAsset>
    {
        /// <summary>
        /// Should the <see cref="RoomController"/> use the RegionDatabase to look up rooms when changing rooms
        /// or use the simple method of 
        /// </summary>
        public bool UseRegionDatabase = false;
        
        public RegionDatabase()
        {
            DatabaseLoadOrder = 0;
        }
        public override void OnRegisterDatabase()
        {
            RegionSystem.RegionDatabase = this;
        }
        
        public override void OnDeregisterDatabase()
        {
            RegionSystem.RegionDatabase = null;
        }
    }
}
