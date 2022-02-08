using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/Stat Equipable Item")]
    public class StatEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private Modifier[] additiveModifiers;
        [SerializeField] private Modifier[] precentageModifiers;
        
        [Serializable]
        struct Modifier
        {
            public Stat Stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach (var modifier in additiveModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (var modifier in precentageModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }
    }
}