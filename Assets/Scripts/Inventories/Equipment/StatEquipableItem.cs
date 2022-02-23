using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "New Equipable Item", menuName = "Inventory/Equipable Item", order = 0)]
    public class StatEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private List<Modifier> additiveModifiers = new List<Modifier>();
        [SerializeField] private List<Modifier> percentageModifiers = new List<Modifier>();
        
        [Serializable]
        private struct Modifier
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
            foreach (var modifier in percentageModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.value;
                }
            }
        }

#if UNITY_EDITOR
        private void AddModifier(List<Modifier> modifierList)
        {
            SetUndo("Add Modifier");
            modifierList?.Add(new Modifier());
            Dirty();
        }

        private void RemoveModifier(List<Modifier>modifierList, int index)
        {
            SetUndo("Remove Modifier");
            modifierList?.RemoveAt(index);
            Dirty();
        }

        private void SetStat(List<Modifier> modifierList, int i, Stat stat)
        {
            if (modifierList[i].Stat == stat) return;
            SetUndo("Change Modifier Stat");
            Modifier mod = modifierList[i];
            mod.Stat = stat;
            modifierList[i] = mod;
            Dirty();
        }
        
        private void SetValue(List<Modifier> modifierList, int i, int newValue)
        {
            if (Math.Abs(modifierList[i].value - newValue) < 0.001f) return;
            SetUndo("Change Modifier Stat");
            Modifier mod = modifierList[i];
            mod.value = newValue;
            modifierList[i] = mod;
            Dirty();
        }
        
        void DrawModifierList(List<Modifier> modifierList)
        {
            int modifierToDelete = -1;
            GUIContent statLabel = new GUIContent("Stat");
            for (int i = 0; i < modifierList.Count; i++)
            {
                Modifier modifier = modifierList[i];
                EditorGUILayout.BeginHorizontal();
                SetStat(modifierList, i, (Stat) EditorGUILayout.EnumPopup(statLabel, modifier.Stat, IsStatSelectable, false));
                SetValue(modifierList, i, EditorGUILayout.IntSlider("Value", (int) modifier.value, -20, 100));
                if (GUILayout.Button("-"))
                {
                    modifierToDelete = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (modifierToDelete > -1)
            {
                RemoveModifier(modifierList, modifierToDelete);
            }

            if (GUILayout.Button("Add Modifier"))
            {
                AddModifier(modifierList);
            }
        }

        bool IsStatSelectable(Enum candidate)
        {
            Stat stat = (Stat) candidate;
            if (stat == Stat.ExperienceReward || stat == Stat.ExperienceToLevelUp) return false;
            return true;
        }
        
        bool drawStatsEquipableItemData = true;
        bool drawAdditive = true;
        bool drawPercentage = true;

        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };
            
            drawStatsEquipableItemData = EditorGUILayout.Foldout(drawStatsEquipableItemData, "Stats Equipable Item Data", true, foldoutStyle);
            if (!drawStatsEquipableItemData) return;

            GUIStyle modifiersFoldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Italic
            };
            
            EditorGUILayout.BeginVertical(contentStyle);
            drawAdditive = EditorGUILayout.Foldout(drawAdditive, "Additive Modifiers", true, modifiersFoldoutStyle);
            if (drawAdditive)
            {
                DrawModifierList(additiveModifiers);
            }
            drawPercentage = EditorGUILayout.Foldout(drawPercentage, "Percentage Modifiers", true, modifiersFoldoutStyle);
            if (drawPercentage)
            {
                DrawModifierList(percentageModifiers);
            }
            EditorGUILayout.EndVertical();
            
        }
#endif
    }
}