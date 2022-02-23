using RPG.Inventories;
using RPG.UI.Dragging;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon icon = null;
        [SerializeField] private EquipLocation equipLocation;

        private Equipment playerEquipment;

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerEquipment.EquipmentUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        public void AddItems(InventoryItem item, int number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem)item);
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            return GetItem() ? 1 : 0;
        }

        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            var equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (!equipableItem.CanEquip(equipLocation, playerEquipment)) return 0;
            if (GetItem() != null) return 0;
            return 1;
        }

        private void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation), MaxAcceptable(GetItem()));
        }
    }
}