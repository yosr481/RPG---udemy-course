using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/Action Item")]
    public class ActionItem : InventoryItem
    {
        [SerializeField] private bool isConsumable = false;

        public bool IsConsumable => isConsumable;

        public virtual void Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
        }
    }
}