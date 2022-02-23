using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience experience;

        private void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<Text>().text = $"XP: {experience.GetExperience():0}";
        }
    }
}