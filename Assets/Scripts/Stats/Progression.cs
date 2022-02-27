using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] private List<ProgressionCharacterClass> progressionCharacterClasses = null;

        private Dictionary<CharacterClass, Dictionary<Stat, List<float>>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            if (!lookupTable[characterClass].ContainsKey(stat)) return 0;
            
            List<float> levels = lookupTable[characterClass][stat];

            if (levels.Count == 0) return 0;
            if(levels.Count < level)
            {
                return levels[levels.Count - 1];
            }

            return levels[level - 1];
        }

        public CharacterClass GetCharacterClassByName(string characterClass)
        {
            return (CharacterClass)Enum.Parse(typeof(CharacterClass), characterClass);
        }

        public List<string> GetStatsNames(CharacterClass characterClass)
        {
            BuildLookup();

            return lookupTable[characterClass].Keys.Select(statKey => Enum.GetName(typeof(Stat), statKey)).ToList();
        }
        
#if UNITY_EDITOR
        public void SetStat(float value, CharacterClass character, Stat stat, int index)
        {
            BuildLookup();
            if (!lookupTable[character].ContainsKey(stat)) return;
            if(FloatEquals(lookupTable[character][stat][index], value)) return;
            Undo.RecordObject(this,"Set Level value");
            lookupTable[character][stat][index] = value;
            EditorUtility.SetDirty(this);
        }
        
        public void AddLevel(CharacterClass character, Stat stat)
        {
            BuildLookup();
            if (!lookupTable[character].ContainsKey(stat)) return;
            Undo.RecordObject(this,"Add Level");
            lookupTable[character][stat].Add(0);
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveLevel(CharacterClass character, Stat stat, int index)
        {
            BuildLookup();
            if (!lookupTable[character].ContainsKey(stat)) return;
            Undo.RecordObject(this,"Remove Level");
            lookupTable[character][stat].RemoveAt(index);
            EditorUtility.SetDirty(this);
        }

        private static bool FloatEquals(float value1, float value2)
        {
            return Math.Abs(value1 - value2) < 0.001f;
        }
#endif
        public int GetLevels(Stat stat, CharacterClass character)
        {
            BuildLookup();
            if (!lookupTable[character].ContainsKey(stat)) return 0;
            return lookupTable[character][stat].Count <= 0 ? 0 : lookupTable[character][stat].Count;
        }

        public int GetStatsNumberForCharacter(CharacterClass characterClass)
        {
            BuildLookup();
            return lookupTable[characterClass].Count;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, List<float>>>();

            foreach (ProgressionCharacterClass progressionClass in progressionCharacterClasses)
            {
                var statLookupTable = new Dictionary<Stat, List<float>>();

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
            public List<ProgressionStat> stats;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            public List<float> levels;
        }
    }
}
