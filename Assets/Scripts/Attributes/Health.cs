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
        [SerializeField] private float regeneratePrecentage = 70;
        [SerializeField] private TakeDamageEvent takeDamage;
        public UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        private LazyValue<float> healthPoints;

        private bool wasDeadLastFrame = false;

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
            return healthPoints.Value <= 0;
        }

        public void TakeDamage(GameObject investigator, float damage)
        {
            healthPoints.Value = Mathf.Max(healthPoints.Value - damage, 0);
            
            if (IsDead())
            {
                UpdateState();
                onDie?.Invoke();
                AwardExperience(investigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
            UpdateState();
        }
        
        public void Heal(float healthChange)
        {
            healthPoints.Value = Mathf.Min(healthPoints.Value + healthChange, GetMaxHealthPoints());
            UpdateState();
        }

        public float GetHealthPoints()
        {
            return healthPoints.Value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthPoints.Value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void UpdateState()
        {
            var animator = GetComponent<Animator>();
            
            if (!wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("Die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            
            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            wasDeadLastFrame = IsDead();
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

            UpdateState();
        }
        
    }
}
