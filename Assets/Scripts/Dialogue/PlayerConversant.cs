using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour, IAction
    {
        [SerializeField] private string playerName;
        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool isChoosing = false;
        private AIConversant targetConversant = null;
        private Dialogue targetDialogue = null;

        public event Action onConversationUpdated;

        private void Update()
        {
            if(!targetConversant) return;

            if (Vector3.Distance(transform.position, targetConversant.transform.position) > 10.0f)
            {
                GetComponent<Mover>().MoveTo(targetConversant.transform.position, 1);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                StartDialogue(targetConversant, targetDialogue);
                targetConversant = null;
            }
        }

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public void StartDialogueAction(AIConversant newConversant, Dialogue newDialogue)
        {
            if(newConversant == currentConversant) return;
            Quit();
            GetComponent<ActionScheduler>().StartAction(this);
            targetConversant = newConversant;
            targetDialogue = newDialogue;
        }

        public void Quit()
        {
            TriggerExitAction();
            currentConversant = null;
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated?.Invoke();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null) return string.Empty;

            return currentNode.GetText();
        }
        
        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }
        
        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated?.Invoke();
                return;
            }
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAiChildren(currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Any();
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null && !string.IsNullOrEmpty(currentNode.GetOnEnterAction()))
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }
        
        private void TriggerExitAction()
        {
            if (currentNode != null && !string.IsNullOrEmpty(currentNode.GetOnExitAction()))
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (string.IsNullOrEmpty(action)) return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
        public void Cancel()
        {
            Quit();
        }
    }
}