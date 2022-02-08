using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectivesContainer;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();
            foreach (Transform item in objectivesContainer)
            {
                Destroy(item.gameObject);
            }
            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = status.IsObjectiveCompleted(objective.reference)
                    ? objectivePrefab
                    : objectiveIncompletePrefab;
                GameObject objectiveInstance = Instantiate(prefab, objectivesContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string rewardString = "";
            foreach (var reward in quest.GetRewards())
            {
                if (!string.IsNullOrEmpty(rewardString))
                {
                    rewardString += ", ";
                }
                rewardString += $"{reward.number.ToString()} {reward.item.GetDisplayName()}";
            }

            if (string.IsNullOrEmpty(rewardString))
            {
                rewardString = "No reward";
            }

            rewardString += ".";
            return rewardString;
        }
    }
}
