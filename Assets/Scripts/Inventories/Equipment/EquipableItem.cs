using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/Equipable Item")]
    public class EquipableItem : InventoryItem
    {
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] private EquipLocation allowedEquipLocation = EquipLocation.Weapon;

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }
    }
}