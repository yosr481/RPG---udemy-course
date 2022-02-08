using System;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    public class UseActionUI : MonoBehaviour
    {
        private GameObject player;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            var index = GetComponentInParent<ActionSlotUI>().GetIndex;
            GetComponent<Button>().onClick.AddListener(() => { player.GetComponent<ActionStore>().Use(index, player); });
        }
    }
}