using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class Store : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string storeName;
        [SerializeField, Range(0,100)] private float sellingPrecentage = 80f;
        [SerializeField] private StockItemConfig[] stockConfig;
        [Serializable]
        private class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0,100)]
            public float buyingDiscountPercentage;
        }

        private Shopper currentShopper;
        private bool isBuyingMode = true;
        private ItemCategory filter = ItemCategory.None;
        
        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        
        public event Action OnChange;

        private void Awake()
        {
            foreach (var config in stockConfig)
            {
                stock[config.item] = config.initialStock;
            }
        }

        public void GetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<StoreItem> GetFilteredItems()
        {
            foreach (StoreItem storeItem in GetAllItems())
            {
                var item = storeItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory == filter)
                {
                    yield return storeItem;
                }
            }
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;
            OnChange?.Invoke();
        }

        public ItemCategory GetFilter() => filter;

        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            OnChange?.Invoke();
        }

        public bool IsBuyingMode() => isBuyingMode;

        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = Inventory.GetPlayerInventory();
            Purse shopperPurse = shopperInventory.GetComponent<Purse>();

            if (shopperInventory == null || shopperPurse == null) return;

            foreach (StoreItem storeItem in GetAllItems())
            {
                InventoryItem item = storeItem.GetInventoryItem();
                int quantity = storeItem.GetQuantityInTransaction();
                float price = storeItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperPurse, price, shopperInventory, item);
                    }
                    else
                    {
                        SellItem(shopperPurse, price, shopperInventory, item);
                    }
                }
            }
            
            OnChange?.Invoke();
        }

        public float TransactionTotal()
        {
            float total = 0;
            foreach (var item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }

            return total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            int availability = GetAvailability(item);
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }
            
            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }
            
            OnChange?.Invoke();
        }

        public string GetStoreName()
        {
            return storeName;
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true;
            
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperPurse == null) return false;

            return shopperPurse.GetBalance() >= TransactionTotal();
        }

        public CursorType GetCursorType()
        {
            return CursorType.Store;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveStore(this);
            }

            return true;
        }


        // Internal Functions
        private void BuyItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {

            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stock[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }
        
        private void SellItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {
            int slot = FindFirstitemSlot(shopperInventory, item);
            if(slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            stock[item]++;
            shopperPurse.UpdateBalance(price);
        }
        
        private int FindFirstitemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                    return i;
            }

            return -1;
        }
        
        private IEnumerable<StoreItem> GetAllItems()
        {
            foreach (var config in stockConfig)
            {
                InventoryItem item = config.item;
                float price = GetPrice(config);
                transaction.TryGetValue(item, out var quantityInTransaction);
                var currentStock = GetAvailability(config.item);
                yield return new StoreItem(item, currentStock, price, quantityInTransaction);
            }
        }

        private float GetPrice(StockItemConfig config)
        {
            if (IsBuyingMode())
            {
                return config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100);
            }
            return config.item.GetPrice() * (sellingPrecentage / 100);
        }
        
        private int GetAvailability(InventoryItem item)
        {
            if (IsBuyingMode())
            {
                return stock[item];
            }

            return CountItemsInInventory(item);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = Inventory.GetPlayerInventory();
            if (inventory == null) return 0;

            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }
        
        private bool HasInventorySpace()
        {
            if (!isBuyingMode) return true;
            
            Inventory shopperInventory = Inventory.GetPlayerInventory();
            if (shopperInventory == null) return false;
            
            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (StoreItem storeItem in GetAllItems())
            {
                InventoryItem item = storeItem.GetInventoryItem();
                int quantity = storeItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        private bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }
    }
}