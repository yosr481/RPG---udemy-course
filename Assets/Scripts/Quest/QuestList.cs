using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Inventories;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        private List<QuestStatus> statuses = new List<QuestStatus>();

        public event Action OnUpdate;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            OnUpdate?.Invoke();
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            OnUpdate?.Invoke();
        }

        public bool IsObjectiveCompleted(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            return status != null && status.IsObjectiveCompleted(objective);
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            return statuses.FirstOrDefault(status => status.GetQuest() == quest);
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;
            statuses.Clear();
            foreach (var objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
            }
            return null;
        }
    }
}
