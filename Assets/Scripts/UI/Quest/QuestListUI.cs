using System;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questPrefab;
        private QuestList questList;

        private void Awake()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.OnUpdate += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        public void Redraw()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI item = Instantiate(questPrefab, transform);
                item.Setup(status);
            }
        }
    }
}
