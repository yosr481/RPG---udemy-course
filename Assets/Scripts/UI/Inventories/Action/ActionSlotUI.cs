using System;
using RPG.Abilities;
using RPG.Inventories;
using RPG.UI.Dragging;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    public class ActionSlotUI : MonoBehaviour,IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] private InventoryItemIcon icon = null;
        [SerializeField] private int index;
        [SerializeField] private Image cooldownOverlay = null;

        private ActionItem item;
        private ActionStore actionStore;
        private CooldownStore cooldownStore;

        // PUBLIC

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            actionStore = player.GetComponent<ActionStore>();
            cooldownStore = player.GetComponent<CooldownStore>();
            actionStore.StoreUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        private void Update()
        {
            cooldownOverlay.fillAmount = cooldownStore.GetFractionRemaining(GetItem());
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