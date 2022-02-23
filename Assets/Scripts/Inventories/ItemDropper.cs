using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();
        private List<DropRecord> otherScenesDroppedItems = new List<DropRecord>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">The number of items to drop</param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, number, GetDropLocation());
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        // PRIVATE

        private void SpawnPickup(InventoryItem item, int number, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializibleVector3 position;
            public int number;
            public int buildSceneIndex;
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new List<DropRecord>();
            var buildSceneIndex = SceneManager.GetActiveScene().buildIndex;
            foreach(Pickup pickup in droppedItems)
            {
                DropRecord droppedItem = new DropRecord();
                droppedItem.itemID = pickup.GetItem().GetItemID();
                droppedItem.position = new SerializibleVector3(pickup.transform.position);
                droppedItem.number = pickup.GetNumber();
                droppedItem.buildSceneIndex = buildSceneIndex;
                droppedItemsList.Add(droppedItem);
            }
            droppedItemsList.AddRange(otherScenesDroppedItems);
            return droppedItemsList;
        }

        void ISaveable.RestoreState(object state)
        {
            var droppedItemsList = (List<DropRecord>)state;
            var buildSceneIndex = SceneManager.GetActiveScene().buildIndex;
            otherScenesDroppedItems.Clear();
            foreach (var item in droppedItemsList)
            {
                if(item.buildSceneIndex != buildSceneIndex)
                {
                    otherScenesDroppedItems.Add(item);
                    continue;
                }
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector();
                int number = item.number;
                SpawnPickup(pickupItem, number, position);
            }
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
} 