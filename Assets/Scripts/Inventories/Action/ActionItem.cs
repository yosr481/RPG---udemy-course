using UnityEditor;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/Action Item")]
    public class ActionItem : InventoryItem
    {
        [SerializeField] private bool isConsumable = false;

        public bool IsConsumable() => isConsumable;

        public virtual bool Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
            return false;
        }

#if UNITY_EDITOR
        void SetIsConsumable(bool value)
        {
            if (isConsumable == value) return;
            SetUndo(value?"Set Consumable":"Set Not Consumable");
            isConsumable = value;
            Dirty();
        }

        bool drawActionItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };
            
            drawActionItem = EditorGUILayout.Foldout(drawActionItem, "Action Item Data", true, foldoutStyle);
            if (!drawActionItem) return;
            
            EditorGUILayout.BeginVertical(contentStyle);
            SetIsConsumable(EditorGUILayout.Toggle("Is Consumable", isConsumable));
            EditorGUILayout.EndVertical();
        }

#endif
    }
}