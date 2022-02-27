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
		
		private List<string> characterTabs = new List<string>();
		private List<string> statTabs = new List<string>();
		private int characterTabSelected = -1;
		private int statTabSelected = -1;
		private CharacterClass selectedCharacter;
		private Stat selectedStat;
		
		[MenuItem("Window/Progression Window")]
		private static void ShowWindow()
		{
			var window = GetWindow<ProgressionEditorWindow>();
			window.titleContent = new GUIContent("Progression Window");
			window.Show();
		}

		public static void ShowEditorWindow(Progression candidate)
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
			characterTabSelected = -1;
			statTabSelected = -1;
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

		private void OnEnable()
		{
			characterTabSelected = -1;
			statTabSelected = -1;
			characterTabs = Enum.GetNames(typeof(CharacterClass)).ToList();
			statTabs = Enum.GetNames(typeof(Stat)).ToList();
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical("Box");
			characterTabSelected = GUILayout.Toolbar(characterTabSelected, characterTabs.ToArray());
			statTabSelected = GUILayout.Toolbar(statTabSelected, statTabs.ToArray());
			EditorGUILayout.EndVertical();
			
			if (characterTabSelected > -1 && statTabSelected > -1)
			{
				selectedCharacter = selected.GetCharacterClassByName(characterTabs[characterTabSelected]);
				selectedStat = (Stat)Enum.Parse(typeof(Stat), statTabs[statTabSelected]);
				
				CreatTableOfLevels();
					
			}
		}

		Vector2 scrollPosition;
		void CreatTableOfLevels()
		{
			int levels = selected.GetLevels(selectedStat, selectedCharacter);
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical("Box");
			
			if (levels > 0)
			{
				for (int x = 0; x < levels; x++)
				{
				
					selected.SetStat(EditorGUILayout.FloatField($"Level {x + 1}",selected.GetStat(
								selectedStat, selectedCharacter, x + 1)
							,GUILayout.MaxWidth(250)),
							selectedCharacter,
							selectedStat, x);
				}
			}
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(" + Add Level", GUILayout.ExpandWidth(false)))
			{
				selected.AddLevel(selectedCharacter, selectedStat);
			}
			if (levels > 0)
			{
				if (GUILayout.Button(" - Delete Level", GUILayout.ExpandWidth(false)))
				{
					selected.RemoveLevel(selectedCharacter, selectedStat, levels - 1);
				}
			}
			
			GUILayout.EndHorizontal();
			
			
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}
}
