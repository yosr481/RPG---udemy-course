using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour, IAction
    {
        [SerializeField] private float storeDistanceRange = 2.0f;

        private Store activeStore;

        public event Action ActiveStoreChange;

        private void Update()
        {
            if(!activeStore) return;

            if (Vector3.Distance(transform.position, activeStore.transform.position) > 10.0f)
            {
                GetComponent<Mover>().MoveTo(activeStore.transform.position, 1);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                ActiveStoreChange?.Invoke();
                activeStore = null;
            }
        }

        private void SetActiveStore(Store store)
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
        }

        public void CloseActiveStore()
        {
            activeStore = null;
            ActiveStoreChange?.Invoke();
        }
        
        public void OpenStoreAction(Store newStore)
        {
            if(newStore == activeStore) return;
            GetComponent<ActionScheduler>().StartAction(this);
            SetActiveStore(newStore);
        }

        public Store GetActiveStore()
        {
            return activeStore;
        }
        public void Cancel()
        {
            GetComponent<Mover>().Cancel();
        }
    }
}
