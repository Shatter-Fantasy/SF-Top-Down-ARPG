using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SF.ItemModule
{
    using DataManagement;
    using Managers;
    
    [Serializable]
    public class PlayerInventory : InventoryBase
    {
        
        //public List<CurrencyData> Currencies = new List<CurrencyData>();
        
        [NonSerialized] public List<ItemData> FilteredConsumable = new List<ItemData>();
        [NonSerialized] public List<Weapon> FilteredWeapons = new List<Weapon>();
        [NonSerialized] public List<Armor> FilteredArmor = new List<Armor>();

  
        //public static Action<CurrencyData> CurrencyPickedUpHandler;
        
        private void Start()
        {
            MetroidvaniaSaveManager.PlayerInventory = this;
        }
        
        public override void AddItem(int itemID, int amount = 1)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            ItemData itemData = new ItemData();

            if (item is WeaponDTO equipmentDTO)
                itemData = (Weapon)equipmentDTO;
            else
                itemData = item;
            
            Items.Add(itemData);
        }

       
        /*
        public void AddItem(CurrencyData currencyData)
        {
            if (Currencies.Count < 1)
                Currencies.Add(currencyData);
            else
                Currencies[0].Quantity += currencyData.Quantity;
        }
        
        public void PickUpItem(CurrencyData currencyData)
        {
            AddItem(currencyData);
            CurrencyPickedUpHandler?.Invoke(currencyData);
        }*/

        public void FilterInventory()
        {
            FilteredConsumable = Items
                .Where(data => data.ItemSubType == ItemSubType.Consumable)
                .ToList();
            
            FilteredWeapons = Items.OfType<Weapon>().ToList();
            FilteredArmor = Items.OfType<Armor>().ToList();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Items Count: {Items.Count} ");
            sb.Append($"Consumables Count: {FilteredConsumable.Count} ");
            sb.Append($"Weapons Count: {FilteredWeapons.Count} ");
            sb.Append($"Armor Count: {FilteredArmor.Count} ");
            return sb.ToString();
        }
    }
}
