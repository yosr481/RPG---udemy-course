using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] Weapon EquippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 10f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHand = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform lefthand, Animator animator)
        {
            DestroyOldWeapon(rightHand, lefthand);

            Weapon weapon = null;

            if (EquippedPrefab != null)
            {
                Transform handPosition = GetHandPosition(rightHand, lefthand);

                weapon = Instantiate(EquippedPrefab, handPosition);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform lefthand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = lefthand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LuanchProjectile(Transform rightHand, Transform lefthand, Health target, GameObject investigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandPosition(rightHand, lefthand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, investigator, calculatedDamage);
        }

        private Transform GetHandPosition(Transform rightHand, Transform lefthand)
        {
            Transform handPosition;

            handPosition = isRightHand ? rightHand : lefthand;
            return handPosition;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}
