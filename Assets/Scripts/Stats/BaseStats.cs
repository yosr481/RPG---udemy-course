using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticles = null;
        [SerializeField] private bool shouldUseModifiers = true;

        public event Action OnLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            var newLevel = CalculateLevel();
            if (newLevel <= currentLevel.Value) return;
            currentLevel.Value = newLevel;
            LevelUpEffect();
            OnLevelUp?.Invoke();
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticles, transform);
        }

        public float GetStat(Stat stat) => (GetBaseStat(stat) + GetAdditiveModifier(stat))
                   * (1 + GetPercentageModifier(stat) / 100);
        
        private float GetBaseStat(Stat stat) => progression.GetStat(stat, characterClass, GetLevel());

        public int GetLevel() => currentLevel.Value;

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            return GetComponents<IModifierProvider>().
                SelectMany(provider => provider.GetAdditiveModifier(stat)).Sum();
        }
        
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            return GetComponents<IModifierProvider>().
                SelectMany(provider => provider.GetPercentageModifier(stat)).Sum();
        }

        private int CalculateLevel()
        {
            // ReSharper disable once InconsistentNaming
            var _experience = GetComponent<Experience>();
            if (_experience == null) return startingLevel;

            var currentXp = _experience.GetExperience();

            var penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (var level = 1; level <= penultimateLevel; level++)
            {
                var xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(xpToLevelUp > currentXp)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}
