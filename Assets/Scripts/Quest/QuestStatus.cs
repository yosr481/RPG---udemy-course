using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private Quest quest;
        private List<string> completedObjectives = new List<string>();

        [System.Serializable]
        private class QuestStatusRecord
        {
            internal string questName;
            internal List<string> completedObjectives;

            public QuestStatusRecord(string questName, List<string> completedObjectives)
            {
                this.questName = questName;
                this.completedObjectives = completedObjectives;
            }
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveCompleted(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public bool IsComplete()
        {
            return quest.GetObjectives().All(objective => completedObjectives.Contains(objective.reference));
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }

        }

        public object CaptureState()
        {
            return new QuestStatusRecord(quest.GetTitle(), completedObjectives);
        }
    }
}