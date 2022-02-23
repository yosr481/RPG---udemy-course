using System;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories
{
    public class EquipableItem : InventoryItem
    {
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] private EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] private Condition equipCondition;

        public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
        {
            return equipLocation == allowedEquipLocation &&
                   equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
        }
        
        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

#if UNITY_EDITOR
        public void SetAllowedEquipLocation(EquipLocation newLocation)
        {
            if (allowedEquipLocation == newLocation) return;
            SetUndo("Change Equip Location");
            allowedEquipLocation = newLocation;
            Dirty();
        }

        private bool drawInventoryItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };

            drawInventoryItem = EditorGUILayout.Foldout(drawInventoryItem, "Equipable Item Data", true, foldoutStyle);
            if(!drawInventoryItem) return;
            
            EditorGUILayout.BeginVertical(contentStyle);
            SetAllowedEquipLocation((EquipLocation)EditorGUILayout.EnumPopup(new GUIContent("Equip Location"), allowedEquipLocation, IsLocationSelectable, false));
            EditorGUILayout.EndVertical();
        }

        public virtual bool IsLocationSelectable(Enum location)
        {
            EquipLocation candidate = (EquipLocation)location;
            return candidate != EquipLocation.Weapon;
        }
#endif
    }
}