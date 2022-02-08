using RPG.UI.Tooltips;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestItemUI>().GetQuest();
            tooltip.GetComponent<QuestTooltipUI>().Setup(status);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}
