using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoint = 0;
        
        public event Action OnExperienceGained;

        public void GainExperience(float experience)
        {
            experiencePoint += experience;
            OnExperienceGained?.Invoke();
        }

        public float GetExperience()
        {
            return experiencePoint;
        }

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            experiencePoint = (float)state;
        }
    }
}
