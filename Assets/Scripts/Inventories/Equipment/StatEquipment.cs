﻿using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null) continue;

                foreach (var modifier in item.GetAdditiveModifier(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null) continue;

                foreach (var modifier in item.GetPercentageModifier(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}