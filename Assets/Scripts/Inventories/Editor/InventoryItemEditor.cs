using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace RPG.Inventories.Editor
{
	public class InventoryItemEditor : EditorWindow
	{
		private InventoryItem selected;
		[MenuItem("Window/InventoryItem Editor")]
		public static void ShowWindow()
		{
			var window = GetWindow<InventoryItemEditor>();
			window.titleContent = new GUIContent("InventoryItem");
			window.Show();
		}

		public static void ShowEditorWindow(InventoryItem candidate)
		{
			InventoryItemEditor window = GetWindow(typeof(InventoryItemEditor), false, "InventoryItem") as InventoryItemEditor;
			if (candidate)
			{
				window.OnSelectionChange();
			}
		}
		private void OnSelectionChange()
		{
			var candidate = EditorUtility.InstanceIDToObject(Selection.activeInstanceID) as InventoryItem;
			if(candidate == null) return;
			selected = candidate;
			Repaint();
		}

		[OnOpenAsset(1)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			InventoryItem candidate = EditorUtility.InstanceIDToObject(instanceID) as InventoryItem;
			if (candidate != null)
			{
				ShowEditorWindow(candidate);
				return true;
			}
			return false;
		}
		
		private void OnGUI()
		{
			if (selected == null)
			{
				EditorGUILayout.HelpBox("No Item Selected", MessageType.Error);
				return;
			}
			
			DrawInspector();
		}

		private Vector2 scrollPosition;
		private void DrawInspector()
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			selected.DrawCustomInspector();
			EditorGUILayout.EndScrollView();
		}
	}
}
