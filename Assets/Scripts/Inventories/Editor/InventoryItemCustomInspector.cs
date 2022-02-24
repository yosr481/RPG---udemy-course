using RPG.Combat;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPG.Inventories.Editor
{
	[CustomEditor(typeof(InventoryItem), true)]
	public class InventoryItemCustomInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor"))
			{
				InventoryItemEditor.ShowEditorWindow((InventoryItem)target);
			}
		}
	}
}
