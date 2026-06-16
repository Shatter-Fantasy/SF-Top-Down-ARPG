using System.Collections.Generic;
using UnityEngine;

namespace SF.ItemModule
{
    using Managers;
    public abstract class ItemContainer<TItemDataType> : MonoBehaviour
    {
        public List<TItemDataType> Items = new List<TItemDataType>();

        public abstract void AddItem(int itemID, int amount = 1);
        public abstract void RemoveItem(int itemID, int amount = 1);
    }

    public class ItemContainer : ItemContainer<ItemData> 
    {
        public override void AddItem(int itemID, int amount = 1)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            
            if (item != null)
            {
                Items.Add(item);
            }
        }
        
        public override void RemoveItem(int itemID, int amount = 1)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            
            if (item != null)
            {
                Items.Remove(item);
            }
        }
    }
}
