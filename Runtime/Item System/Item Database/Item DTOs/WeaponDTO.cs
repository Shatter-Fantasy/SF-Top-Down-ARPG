using UnityEngine;

namespace SF.ItemModule
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "SF/Item System/Item Data/Weapon Data")]
    public class WeaponDTO : ItemDTO
    {
        public Weapon WeaponData;
    } 
}
