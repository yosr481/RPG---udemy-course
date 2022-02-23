using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            GetComponent<Text>().text = $"Health: {health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}";
        }
    }
}