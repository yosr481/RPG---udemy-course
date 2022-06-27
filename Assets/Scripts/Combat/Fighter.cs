using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;
using RPG.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float timeBetweenAttacks = 1.21f;
        [SerializeField] private Transform rightHandPosition = null;
        [SerializeField] private Transform leftHandPosition = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;
        [SerializeField] private float autoAttackRange = 4f;

        private float timeSinceLastAttack = Mathf.Infinity;
        private Health target;
        private Equipment equipment;
        private Mover mover;
        private Animator animator;
        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.EquipmentUpdated += UpdateWeapon;
            }
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            currentWeaponConfig = defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead())
            {
                target = FindNewTargetInRange();
                if(target == null) return;
            }

            if (!IsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(currentWeaponConfig);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.Value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            EquipWeapon(weapon == null ? defaultWeapon : weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandPosition, leftHandPosition, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            return isRightHand ? rightHandPosition : leftHandPosition;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                // Triggering the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }
        
        private Health FindNewTargetInRange()
        {
            Health currentCandidate = null;
            float bestDistance = Mathf.Infinity;
            
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(
                    transform.position, candidate.transform.position);
                if (candidateDistance < bestDistance)
                {
                    currentCandidate = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return currentCandidate;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            // Sphere cast
            var hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up, autoAttackRange);
            foreach (var hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if(!health) continue;
                if(health.IsDead()) continue;
                if(health.gameObject == gameObject) continue;
                yield return health;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("StopAttack");
            animator.SetTrigger("Attack");
        }

        // Animation Event
        private void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            var targetBaseStat = target.GetComponent<BaseStats>();
            if(targetBaseStat != null)
            {
                float defence = targetBaseStat.GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if(currentWeapon.Value != null)
            {
                currentWeapon.Value.OnHit(); 
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LuanchProjectile(rightHandPosition, leftHandPosition, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);

            }
        }

        private void Shoot()
        {
            Hit();
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }
    }
}
