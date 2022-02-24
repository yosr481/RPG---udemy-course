using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
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
            var equippedItemForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var item in equippedItems)
            {
                equippedItemForSerialization[item.Key] = item.Value.GetItemID();
            }

            return equippedItemForSerialization;
        }

        public void RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();
            var equippedItemForSerialization = (Dictionary<EquipLocation, string>)state;
            foreach (var pair in equippedItemForSerialization)
            {
                var item = (EquipableItem) InventoryItem.GetFromID(pair.Value);
                if (item) equippedItems[pair.Key] = item;
            }

            EquipmentUpdated?.Invoke();
        }
        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            if (predicate == EPredicate.HasItemEquipped)
            {
                return equippedItems.Values.Any(item => item.GetItemID() == parameters[0]);
            }

            return null;
        }
    }
}