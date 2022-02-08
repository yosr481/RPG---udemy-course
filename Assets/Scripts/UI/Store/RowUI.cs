using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI NameText;
        [SerializeField] private TextMeshProUGUI availabilityText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI quantityText;

        private Store currentStore;
        private StoreItem item;
        public void Setup(Store currentStore, StoreItem item)
        {
            this.currentStore = currentStore;
            this.item = item;
            iconImage.sprite = item.GetIcon();
            NameText.text = item.GetName();
            availabilityText.text = item.GetAvailability().ToString();
            priceText.text = CultureInfo.GetCultureInfoByIetfLanguageTag("he").NumberFormat.CurrencySymbol + item.GetPrice().ToString("N");
            quantityText.text = item.GetQuantityInTransaction().ToString();
        }

        public void Add()
        {
            currentStore.AddToTransaction(item.GetInventoryItem(), 1);
        }

        public void Subtract()
        {
            currentStore.AddToTransaction(item.GetInventoryItem(), -1);
        }
    }
}
