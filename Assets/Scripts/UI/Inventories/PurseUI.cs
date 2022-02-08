using System;
using System.Globalization;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI purseText;

        private Purse playerPurse;

        private void Start()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
            playerPurse.onChange += RefreshUI;
            RefreshUI();
        }

        public void RefreshUI()
        {
            purseText.text = CultureInfo.GetCultureInfoByIetfLanguageTag("he").NumberFormat.CurrencySymbol +
                             playerPurse.GetBalance().ToString("N");
        }
    }
}