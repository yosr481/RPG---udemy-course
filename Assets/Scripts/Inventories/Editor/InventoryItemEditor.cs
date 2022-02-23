using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Inventories.Editor
{
	public class InventoryItemEditor : EditorWindow
	{
		private InventoryItem selected;
		[MenuItem("Window/InventoryItem Editor")]
		private static void ShowWindow()
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
		
		GUIStyle previewStyle;
		GUIStyle descriptionStyle;
		GUIStyle headerStyle;

		void OnEnable()
		{
			previewStyle = new GUIStyle
			{
				normal =
				{
					background = EditorGUIUtility.Load(
						"Assets/Asset packs/GUI PRO Kit - Casual Game/ResourcesData/Sprite/Component/Frame/Frame_ItemFrame01_Color_Purple.png") as Texture2D
				},
				padding = new RectOffset(40, 40, 40, 40),
				border = new RectOffset(0, 0, 0, 0)
			};
		}

		private bool stylesInitialized = false;


		private void OnGUI()
		{
			if (selected == null)
			{
				EditorGUILayout.HelpBox("No Item Selected", MessageType.Error);
				return;
			}

			if (!stylesInitialized)
			{
				descriptionStyle = new GUIStyle(GUI.skin.label)
				{
					richText = true,
					wordWrap = true,
					stretchHeight = true,
					fontSize = 14,
					alignment = TextAnchor.MiddleCenter
				};
				headerStyle = new GUIStyle(descriptionStyle)
				{
					fontSize = 24
				};
				stylesInitialized = true;
			}
			DrawInspector();
		}

		private Vector2 scrollPosition;
		private void DrawInspector()
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			selected.DrawCustomInspector();
			GUILayout.EndScrollView();
		}
	}
}
