using RPG.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat.UI
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if(fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "Enemy Health: N/A";
                return;
            }

            Health health = fighter.GetTarget();
            GetComponent<Text>().text = $"Enemy Health: {health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}";
        }
    }
}