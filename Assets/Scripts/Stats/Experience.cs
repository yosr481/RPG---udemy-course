using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoint = 0;
        
        public event Action OnExperienceGained;
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                GainExperience(Time.deltaTime * 100);
            }
        }

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
