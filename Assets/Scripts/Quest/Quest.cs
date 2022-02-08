using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "New Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Objective> objectives = new List<Objective>();
        [SerializeField] private List<Reward> rewards = new List<Reward>();

        [System.Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }
        
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }
        
        public string GetTitle()
        {
            return name;
        }

        public int GetObjectivesCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            return objectives.Any(objective => objective.reference == objectiveRef);
        }

        public static Quest GetByName(string questName)
        {
            return Resources.LoadAll<Quest>("").FirstOrDefault(quest => quest.name == questName);
        }
    }
}
