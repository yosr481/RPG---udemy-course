using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class ActionStore : MonoBehaviour, ISaveable
    {
        private Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();
        private class DockedItemSlot
        {
            public ActionItem item;
            public int number;
        }

        public event Action StoreUpdated;

        public ActionItem GetItem(int index)
        {
            return dockedItems.ContainsKey(index) ? dockedItems[index].item : null;
        }

        public int GetNumber(int index)
        {
            return dockedItems.ContainsKey(index) ? dockedItems[index].number : 0;
        }

        public void AddAction(InventoryItem item, int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                if (object.ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot()
                {
                    item = item as ActionItem,
                    number = number
                };
                dockedItems[index] = slot;
            }
            
            StoreUpdated?.Invoke();
        }

        public bool Use(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                var wasUsed = dockedItems[index].item.Use(user);
                if (wasUsed && dockedItems[index].item.IsConsumable())
                {
                    RemoveItem(index, 1);
                }

                return true;
            }

            return false;
        }

        public void RemoveItem(int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;
                if (dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }
                StoreUpdated?.Invoke();
            }
        }

        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;
            if (dockedItems.ContainsKey(index) && !ReferenceEquals(item, dockedItems[index].item))
            {
                return 0;
            }

            if (actionItem.IsConsumable()) return int.MaxValue;
            if (dockedItems.ContainsKey(index)) return 0;
            return 1;
        }
        
        [Serializable]
        private struct DockedItemRecord
        {
            public string ItemID;
            public int Number;
        }
        
        public object CaptureState()
        {
            var state = new Dictionary<int, DockedItemRecord>();
            foreach (var pair in dockedItems)
            {
                var record = new DockedItemRecord();
                record.ItemID = pair.Value.item.GetItemID();
                record.Number = pair.Value.number;
                state[pair.Key] = record;
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var stateDict = (Dictionary<int, DockedItemRecord>) state;
            foreach (var pair in stateDict)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.ItemID), pair.Key, pair.Value.Number);
            }
        }
    }

}
