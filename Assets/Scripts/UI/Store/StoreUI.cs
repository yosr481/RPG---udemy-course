using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class StoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI storeName;
        [SerializeField] private Transform listRoot;
        [SerializeField] private RowUI rowPrefab;
        [SerializeField] private TextMeshProUGUI totalText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button switchButton;
        
        private Shopper shopper;
        private Store currentStore;
        private Color originalTotalTextColor;
        private FilterButtonUI[] filterButtons;

        private void Awake()
        {
            filterButtons = GetComponentsInChildren<FilterButtonUI>();
        }

        private void Start()
        {
            originalTotalTextColor = totalText.color;
            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper) shopper.ActiveStoreChange += StoreChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);
            StoreChanged();
        }

        private void StoreChanged()
        {
            if (currentStore)
            {
                currentStore.OnChange -= RefreshUI;
            }
            currentStore = shopper.GetActiveStore();
            gameObject.SetActive(currentStore != null);

            foreach (var button in filterButtons)
            {
                button.SetStore(currentStore);
            }
            
            if (!currentStore) return;
            storeName.text = currentStore.GetStoreName();
            currentStore.OnChange += RefreshUI;
            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }
            
            foreach (StoreItem item in currentStore.GetFilteredItems())
            {
                RowUI row = Instantiate(rowPrefab, listRoot);
                row.Setup(currentStore, item);
            }

            totalText.text = currentStore.TransactionTotal().ToString("N");
            totalText.color = currentStore.HasSufficientFunds() ? originalTotalTextColor : Color.red;
            confirmButton.interactable = currentStore.CanTransact();
            TextMeshProUGUI confirmButtonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI switchButtonText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            switchButtonText.text = currentStore.IsBuyingMode() ? "Switch to Selling" : "Switch to Buying";
            confirmButtonText.text = currentStore.IsBuyingMode() ? "Buy" : "Sell";
            
            foreach (var button in filterButtons)
            {
                button.RefreshUI();
            }
        }

        public void ConfirmTransaction()
        {
            currentStore.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentStore.SelectMode(!currentStore.IsBuyingMode());
        }

        public void Close()
        {
            shopper.CloseActiveStore();
        }
    }
}
