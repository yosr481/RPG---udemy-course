using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Shops
{
    public class Store : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] private string storeName;
        [SerializeField, Range(0,100)] private float sellingPercentage = 80f;
        [SerializeField] private float maximumBarterDiscount = 80f;
        [SerializeField] private StockItemConfig[] stockConfig;
        [Serializable]
        private class StockItemConfig
        {
            public InventoryItem Item;
            public int InitialStock;
            [Range(0,100)]
            public float BuyingDiscountPercentage;
            public int LevelToUnlock = 0;
        }

        private Shopper currentShopper;
        private bool isBuyingMode = true;
        private ItemCategory filter = ItemCategory.None;
        
        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        
        public event Action OnChange;

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

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
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
        
        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            
            foreach (var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }
        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>) state;

            stockSold.Clear();
            foreach (var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }


        // Internal Functions
        private void BuyItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {

            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }
                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }
        
        private void SellItem(Purse shopperPurse, float price, Inventory shopperInventory, InventoryItem item)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if(slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }
        
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
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
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            
            foreach (var item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) continue;

                float price = prices[item];
                transaction.TryGetValue(item, out var quantityInTransaction);
                var currentStock = availabilities[item];
                yield return new StoreItem(item, currentStock, price, quantityInTransaction);
            }
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

        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;

            return stats.GetLevel();
        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.Item))
                    {
                        stockSold.TryGetValue(config.Item, out var soldAmount);
                        availabilities[config.Item] = -soldAmount;
                    }
                
                    availabilities[config.Item] += config.InitialStock;
                }
                else
                {
                    availabilities[config.Item] = CountItemsInInventory(config.Item);
                }
            }
            
            return availabilities;
        }
        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!prices.ContainsKey(config.Item))
                    {
                        prices[config.Item] = config.Item.GetPrice() * GetBarterDiscount();
                    }

                    prices[config.Item] *= 1 - config.BuyingDiscountPercentage / 100;
                }
                else
                {
                    prices[config.Item] = config.Item.GetPrice() * (sellingPercentage / 100);
                }
            }
            return prices;
        }
        
        private float GetBarterDiscount()
        {
            var baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return 1 - Mathf.Min(discount, maximumBarterDiscount) / 100;
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();

            foreach (var config in stockConfig)
            {
                if(config.LevelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }
        
        private bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }
    }
}