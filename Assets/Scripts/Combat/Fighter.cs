using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using RPG.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacks = 1.21f;
        [SerializeField] Transform rightHandPosition = null;
        [SerializeField] Transform leftHandPosition = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

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
            if (target.IsDead()) return;

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

        private void TriggerAttack()
        {
            animator.ResetTrigger("StopAttack");
            animator.SetTrigger("Attack");
        }

        // Animation Event
        void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

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

        void Shoot()
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

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
