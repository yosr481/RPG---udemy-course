using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver, IHasItemID
    {
        // CONFIG DATA
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField]
        private string itemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField]
        private string displayName = null;
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField][TextArea]
        private string description = null;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField]
        private Sprite icon = null;
        [Tooltip("The prefab that should be spawned when this item is dropped.")]
        [SerializeField]
        private Pickup pickup = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField]
        private bool stackable = false;
        [SerializeField] private float price;
        [SerializeField] private ItemCategory category = ItemCategory.None;

        [NonSerialized] protected GUIStyle contentStyle;

        // STATE
        private static Dictionary<string, InventoryItem> _itemLookupCache;

        // PUBLIC

        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            return ResourceRetriever<InventoryItem>.GetFromID(itemID);
        }

        /// <summary>
        /// Spawn the pickup gameobject into the world.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <param name="number">The number of items to pickup.</param>
        /// <returns>Reference to the pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var spawnPickup = Instantiate(pickup);
            spawnPickup.transform.position = position;
            spawnPickup.Setup(this, number);
            return spawnPickup;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        public Pickup GetPickup()
        {
            return pickup;
        }

        public float GetPrice()
        {
            return price;
        }

        public ItemCategory GetCategory => category;

#if UNITY_EDITOR
        private bool drawInventoryItem = true;
        public virtual void DrawCustomInspector()
        {
            contentStyle = new GUIStyle
            {
                padding = new RectOffset(15,15,0,0)
            };
            
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };

            drawInventoryItem = EditorGUILayout.Foldout(drawInventoryItem, "Inventory Item Data", true, foldoutStyle);
            if(!drawInventoryItem) return;

            EditorGUILayout.BeginVertical(contentStyle);
            SetItemID(EditorGUILayout.TextField("ItemID (clear to reset)", GetItemID()));
            SetDisplayName(EditorGUILayout.TextField("Display name", GetDisplayName()));
            SetDescription(EditorGUILayout.TextField("Description", GetDescription()));
            SetIcon((Sprite)EditorGUILayout.ObjectField("Icon", GetIcon(), typeof(Sprite), false));
            SetPickup((Pickup)EditorGUILayout.ObjectField("Pickup", GetPickup(), typeof(Pickup), false));
            SetStackable(EditorGUILayout.Toggle("Stackable", IsStackable()));
            EditorGUILayout.EndVertical();
        }
        
        // Setters
        private void SetItemID(string newItemID)
        {
            if(itemID==newItemID) return;
            SetUndo("Change ItemID");
            itemID=newItemID;
            Dirty();
        }

        private void SetDisplayName(string newDisplayName)
        {
            if (newDisplayName == displayName) return;
            SetUndo("Change Display Name");
            displayName = newDisplayName;
            Dirty();
        }

        private void SetDescription(string newDescription)
        {
            if (newDescription == description) return;
            SetUndo("Change Description");
            description = newDescription;
            Dirty();
        }

        private void SetIcon(Sprite newIcon)
        {
            if (icon == newIcon) return;
            SetUndo("Change Icon");
            icon = newIcon;
            Dirty();
        }

        private void SetPickup(Pickup newPickup)
        {
            if (pickup == newPickup) return;
            SetUndo("Change Pickup");
            pickup = newPickup;
            Dirty();
        }

        private void SetStackable(bool newStackable)
        {
            if (stackable == newStackable) return;
            SetUndo(stackable?"Set Not Stackable": "Set Stackable");
            stackable = newStackable;
            Dirty();
        }
        
        // Helper functions
        protected void SetUndo(string message)
        {
            Undo.RecordObject(this, message);
        }

        protected void Dirty()
        {
            EditorUtility.SetDirty(this);
        }

        protected bool FloatEquals(float value1, float value2)
        {
            return Math.Abs(value1 - value2) < 0.001f;
        }
#endif
        
        // PRIVATE
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}
