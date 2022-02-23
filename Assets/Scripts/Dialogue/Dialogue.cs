using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "New Dialogue")]
    public class Dialogue: ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
        
        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childID in parentNode.GetChildren())
            {
                if(nodeLookup.ContainsKey(childID)) yield return nodeLookup[childID];
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            var newNode = MakeNode(parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added dialogue node.");
            
            AddNode(newNode);
        }

        private static DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                newNode.SetPosition(new Vector2(parentNode.GetRect().xMax + 10, parentNode.GetRect().center.y - 42.5f));
                parentNode.AddChild(newNode.name);
                newNode.SetIsPlayerSpeaking(!parentNode.IsPlayerSpeaking());
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }
        
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted dialogue node.");
            nodes.Remove(nodeToDelete);
            OnValidate();
            
            foreach (var node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                var newNode = MakeNode(null);
                AddNode(newNode);
            }
            
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (var node in GetAllNodes())
                {
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAiChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
    }
}