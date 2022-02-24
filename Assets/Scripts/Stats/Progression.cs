using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] progressionCharacterClasses = null;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            if (!lookupTable[characterClass].ContainsKey(stat)) return 0;
            
            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length == 0) return 0;
            if(levels.Length < level)
            {
                return levels[levels.Length - 1];
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            if (!lookupTable[characterClass].ContainsKey(stat))
                return 0;
            
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        public int GetStatsNumberForCharacter(CharacterClass character)
        {
            BuildLookup();
            return lookupTable[character].Count;
        }

        public int GetNumberOfMaxLevelsInStats(CharacterClass character)
        {
            return (from progressionClass in progressionCharacterClasses where progressionClass.characterClass == character select progressionClass.stats.Max(stat => stat.levels.Length - 1)).FirstOrDefault();
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in progressionCharacterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        public class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
