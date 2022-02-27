using RPG.Inventories;
using RPG.Inventories.Editor;
using UnityEditor;
using UnityEngine;

namespace RPG.Stats.Editor
{
	[CustomEditor(typeof(Progression))]
	public class ProgressionCustomInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor"))
			{
				ProgressionEditorWindow.ShowEditorWindow((Progression)target);
			}
		}
	}
}
