using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title, progression;

        private QuestStatus status;
        public void Setup(QuestStatus status)
        {
            this.status = status;
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();
            progression.text = status.GetCompletedCount().ToString() + "/" + quest.GetObjectivesCount().ToString();
        }

        public QuestStatus GetQuest()
        {
            return status;
        }
    }
}
