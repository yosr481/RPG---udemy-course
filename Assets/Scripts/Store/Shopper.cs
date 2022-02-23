using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        [SerializeField] private float storeDistanceRange = 2.0f;

        private Store activeStore;

        public event Action ActiveStoreChange;

        public void SetActiveStore(Store store)
        {
            if (activeStore)
            {
                activeStore.GetShopper(null);
            }
            
            activeStore = store;
            if (activeStore)
            {
                activeStore.GetShopper(this);
            }
            
            ActiveStoreChange?.Invoke();
        }

        public Store GetActiveStore()
        {
            return activeStore;
        }
    }
}
