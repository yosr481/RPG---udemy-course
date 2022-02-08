using RPG.Inventories;
using RPG.UI.Dragging;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class ActionSlotUI : MonoBehaviour,IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index;
        
        ActionItem item;
        ActionStore actionStore;

        // PUBLIC

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            actionStore = player.GetComponent<ActionStore>();
            actionStore.StoreUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        public void AddItems(InventoryItem item, int number)
        {
            actionStore.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return actionStore.GetItem(index);
        }

        public int GetNumber()
        {
            return actionStore.GetNumber(index);
        }
        
        public int GetIndex =>  index;

        public void RemoveItems(int number)
        {
            actionStore.RemoveItem(index, number);
        }

        public int MaxAcceptable(InventoryItem _item)
        {
            return actionStore.MaxAcceptable(_item, index);
        }

        private void RedrawUI()
        {
            icon.SetItem(GetItem(), GetNumber());
        }
    }
}