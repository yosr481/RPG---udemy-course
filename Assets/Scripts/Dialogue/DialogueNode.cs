using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 85);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private Condition condition;

        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move dialogue node.");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update dialogue text.");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add dialogue link.");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove dialogue link.");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetIsPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change dialogue speaker.");
            this.isPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}