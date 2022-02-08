using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Quests;
using UnityEngine;

public class ClickableBulding : MonoBehaviour, IRaycastable
{
    private QuestCompletion questCompletion;

    private void Awake()
    {
        questCompletion = GetComponent<QuestCompletion>();
    }

//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("House of food has been found!");
//        questCompletion.CompleteObjective();
//        Destroy(gameObject);
//    }

    public CursorType GetCursorType()
    {
        if (questCompletion.PlayerHasQuest())
        {
            if (!questCompletion.IsObjectiveCompleted())
                return CursorType.Pickup;
        }

        return CursorType.None;
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!questCompletion.PlayerHasQuest() || questCompletion.IsObjectiveCompleted()) return false;
            Debug.Log("House of food has been found!");
            GetComponent<QuestCompletion>().CompleteObjective();
            Destroy(gameObject);
        }

        return true;
    }
}
