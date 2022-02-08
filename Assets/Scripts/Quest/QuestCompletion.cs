using System;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;
        
        QuestList questList;

        private void Awake()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        }

        // Start is called before the first frame update
        public void CompleteObjective()
        {
            questList.CompleteObjective(quest, objective);
        }

        public bool IsObjectiveCompleted()
        {
            return questList.IsObjectiveCompleted(quest, objective);
        }

        public bool PlayerHasQuest()
        {
            return questList.HasQuest(quest);
        }
    }
}
