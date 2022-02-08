using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        private Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        public event Action EquipmentUpdated;

        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            return !equippedItems.ContainsKey(equipLocation) ? null : equippedItems[equipLocation];
        }

        public void AddItem(EquipLocation equipLocation, EquipableItem item)
        {
            equippedItems.Add(equipLocation, item);
            EquipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation equipLocation)
        {
            equippedItems.Remove(equipLocation);
            EquipmentUpdated?.Invoke();
        }

        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        } 

        public object CaptureState()
        {
            var equipedItemForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var item in equippedItems)
            {
                equipedItemForSerialization[item.Key] = item.Value.GetItemID();
            }

            return equipedItemForSerialization;
        }

        public void RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();
            var equipedItemForSerialization = (Dictionary<EquipLocation, string>)state;
            foreach (var pair in equipedItemForSerialization)
            {
                var item = (EquipableItem) InventoryItem.GetFromID(pair.Value);
                if (item) equippedItems[pair.Key] = item;
            }
        }
    }
}