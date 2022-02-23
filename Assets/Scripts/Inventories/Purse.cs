using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour,IItemStore, ISaveable
    {
        [SerializeField] private float startingBalance = 1000f;

        private float balance = 0;

        public event Action onChange;

        private void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            onChange?.Invoke();
        }
        public object CaptureState()
        {
            return balance;
        }
        public void RestoreState(object state)
        {
            balance = (float)state;
        }
        public int AddItems(InventoryItem item, int number)
        {
            if (!(item is CurrencyItem)) return 0;
            UpdateBalance(item.GetPrice() * number);
            return number;
        }
    }
}