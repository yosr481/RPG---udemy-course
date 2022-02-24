using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Stats.Editor
{
	public class ProgressionEditorWindow : EditorWindow
	{
		private Progression selected;
		
		[MenuItem("Window/Progression Window")]
		private static void ShowWindow()
		{
			var window = GetWindow<ProgressionEditorWindow>();
			window.titleContent = new GUIContent("Progression Window");
			window.Show();
		}

		private static void ShowEditorWindow(Progression candidate)
		{
			ProgressionEditorWindow window = GetWindow(typeof(ProgressionEditorWindow), false, "Progression Window") as ProgressionEditorWindow;
			if (candidate)
			{
				if (window != null) window.OnSelectionChange();
			}
		}
		
		private void OnSelectionChange()
		{
			var candidate = EditorUtility.InstanceIDToObject(Selection.activeInstanceID) as Progression;
			if(candidate == null) return;
			selected = candidate;
			Repaint();
		}
		
		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			Progression candidate = EditorUtility.InstanceIDToObject(instanceID) as Progression;
			if (candidate != null)
			{
				ShowEditorWindow(candidate);
				return true;
			}
			return false;
		}
		private string[] tabs;
		private int tabSelected = -1;
		private CharacterClass selectedCharacter;
		
		private void Awake()
		{
			tabs = Enum.GetNames(typeof(CharacterClass));
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical("Box");
			tabSelected = GUILayout.Toolbar(tabSelected, tabs);
			EditorGUILayout.EndVertical();

			if (tabSelected > -1)
			{
				selectedCharacter = (CharacterClass)Enum.Parse(typeof(CharacterClass), tabs[tabSelected]);
				CreatTableOfLevels();
			}
		}

		Vector2 scrollPosition;
		void CreatTableOfLevels()
		{
			int stats = selected.GetStatsNumberForCharacter(selectedCharacter);
			string[] statNames = Enum.GetNames(typeof(Stat));

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			
			GUILayout.BeginVertical("Box");
			
			for (int y = 0; y < stats; y++)
			{
				Stat currentStat = (Stat)Enum.Parse(typeof(Stat), statNames[y]);
				int levels = selected.GetLevels(currentStat, selectedCharacter);
				
				GUILayout.BeginHorizontal();

				if (levels > 0)
				{
					EditorGUILayout.LabelField(statNames[y],GUILayout.ExpandWidth(false));
			
					for (int x = 0; x < levels; x++)
					{
						EditorGUILayout.FloatField(selected.GetStat(currentStat, selectedCharacter, x + 1),GUILayout.ExpandWidth(false),GUILayout.MaxWidth(50));
					}
				}
			
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndScrollView();
		}
	}
}
