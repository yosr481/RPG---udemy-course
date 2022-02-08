using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regeneratePrecentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }
        
        LazyValue<float> healthPoints;

        bool isDead = false;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject investigator, float damage)
        {
            healthPoints.Value = Mathf.Max(healthPoints.Value - damage, 0);
            
            if (healthPoints.Value <= 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(investigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public float GetHealthPoints()
        {
            return healthPoints.Value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPrecentage()
        {
            return 100 * GetFracction();
        }

        public float GetFracction()
        {
            return healthPoints.Value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject investigator)
        {
            Experience experience = investigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regeneratePrecentage / 100);
            healthPoints.Value = Mathf.Max(healthPoints.Value, regenHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.Value;
        }

        public void RestoreState(object state)
        {
            healthPoints.Value = (float)state;

            if(healthPoints.Value <= 0)
            {
                Die();
            }
        }
    }
}
