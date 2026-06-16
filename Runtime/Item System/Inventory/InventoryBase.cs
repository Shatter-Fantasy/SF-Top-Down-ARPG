using System;

namespace SF.ItemModule
{
    public class InventoryBase : ItemContainer
    {
        
        /// <summary>
        /// Invoked event when an item has been picked up off the ground.
        /// Does not activate when gaining items from shops, quest rewards, or from NPC dialogue interactions.
        /// </summary>
        public static Action<int> ItemPickedUpHandler;
        
        public virtual void PickUpItem(int itemID)
        {
            AddItem(itemID);
            ItemPickedUpHandler?.Invoke(itemID);
        }
    }
}
