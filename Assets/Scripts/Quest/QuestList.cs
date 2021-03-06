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

        private void Update()
        {
            CompeteObjectivesByPredicates();
        }
        
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
        
        private void CompeteObjectivesByPredicates()
        {
            foreach (var status in statuses)
            {
                if(status.IsComplete()) continue;
                var quest = status.GetQuest();
                foreach (var objective in quest.GetObjectives())
                {
                    if(status.IsObjectiveCompleted(objective.reference)) continue;
                    if(!objective.usesCondition) continue;
                    if(objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                        CompleteObjective(quest, objective.reference);
                }
            }
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

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));
                case EPredicate.CompletedQuest:
                    var questStatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    return questStatus != null && questStatus.IsComplete();
                case EPredicate.CompletedObjective:
                    var status = GetQuestStatus(Quest.GetByName(parameters[0]));
                    return status != null && status.IsObjectiveCompleted(parameters[1]);
            }
            return null;
        }
    }
}
