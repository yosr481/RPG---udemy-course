using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private GUIStyle playerNodeStyle;
        [NonSerialized] private DialogueNode draggingNode = null;
        [NonSerialized] private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingParentNode = null;
        private Vector2 scrollPosition;
        [NonSerialized] private bool dragginCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;
        private const float canvasSize = 4000;
        private const float backgroundSize = 50;
        
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogue(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectedChanged;
            
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20,20,20,20);
            nodeStyle.border = new RectOffset(12,12,12,12);
            
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20,20,20,20);
            playerNodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnSelectedChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No selected dialogue.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;

                float multiplier = canvasSize / backgroundSize;
                Rect texCoords = new Rect(0,0,multiplier,multiplier);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);
                
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    dragginCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && dragginCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && dragginCanvas)
            {
                dragginCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            var guiStyle = node.IsPlayerSpeaking() ? playerNodeStyle : nodeStyle;
            GUILayout.BeginArea(node.GetRect(), guiStyle);
           
            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            DrawLinkButtons(node);
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if(linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (var childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlOffset = endPosition - startPosition;
                controlOffset.y = 0;
                controlOffset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlOffset, 
                    endPosition - controlOffset, 
                    Color.white,null,4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 currentMousePosition)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(currentMousePosition))
                {
                    foundNode = node;
                }   
            }

            return foundNode;
        }
    }
}