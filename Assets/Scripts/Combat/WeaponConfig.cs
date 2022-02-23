using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private Weapon EquippedPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 10f;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private bool isRightHand = true;
        [SerializeField] private Projectile projectile = null;

        private const string weaponName = "Weapon";

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

#if UNITY_EDITOR
        private void SetWeaponRange(float newWeaponRange)
        {
            if (FloatEquals(weaponRange, newWeaponRange)) return;
            SetUndo("Set Weapon Range");
            weaponRange = newWeaponRange;
            Dirty();
        }

        private void SetWeaponDamage(float newWeaponDamage)
        {
            if (FloatEquals(weaponDamage, newWeaponDamage)) return;
            SetUndo("Set Weapon Damage");
            weaponDamage = newWeaponDamage;
            Dirty();
        }

        private void SetPercentageBonus(float newPercentageBonus)
        {
            if (FloatEquals(percentageBonus, newPercentageBonus)) return;
            SetUndo("Set Percentage Bonus");
            percentageBonus = newPercentageBonus;
            Dirty();
        }

        private void SetIsRightHanded(bool newRightHanded)
        {
            if (isRightHand == newRightHanded) return;
            SetUndo(newRightHanded?"Set as Right Handed":"Set as Left Handed");
            isRightHand = newRightHanded;
            Dirty();
        }

        private void SetAnimatorOverride(AnimatorOverrideController newOverride)
        {
            if (newOverride == animatorOverride) return;
            SetUndo("Change AnimatorOverride");
            animatorOverride = newOverride;
            Dirty();
        }

        private void SetEquippedPrefab(Weapon newWeapon)
        {
            if (newWeapon == EquippedPrefab) return;
            SetUndo("Set Equipped Prefab");
            EquippedPrefab = newWeapon;
            Dirty();
        }

        private void SetProjectile(Projectile possibleProjectile)
        {
            //if (!possibleProjectile.TryGetComponent<Projectile>(out Projectile newProjectile)) return;
            if (possibleProjectile == projectile) return;
            SetUndo("Set Projectile");
            projectile = possibleProjectile;
            Dirty();
        }
        
        public override bool IsLocationSelectable(Enum location)
        {
            EquipLocation candidate = (EquipLocation) location;
            return candidate == EquipLocation.Weapon;
        }

        private bool drawInventoryItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };

            drawInventoryItem = EditorGUILayout.Foldout(drawInventoryItem, "Weapon Config Data", true, foldoutStyle);
            if(!drawInventoryItem) return;

            EditorGUILayout.BeginVertical(contentStyle);
            SetEquippedPrefab((Weapon)EditorGUILayout.ObjectField("Equipped Prefab", EquippedPrefab,typeof(Object), false));
            SetWeaponDamage(EditorGUILayout.Slider("Weapon Damage", weaponDamage, 0, 100));
            SetWeaponRange(EditorGUILayout.Slider("Weapon Range", weaponRange, 1,40));
            SetPercentageBonus(EditorGUILayout.IntSlider("Percentage Bonus", (int)percentageBonus, -10, 100));
            SetIsRightHanded(EditorGUILayout.Toggle("Is Right Handed", isRightHand));
            SetAnimatorOverride((AnimatorOverrideController)EditorGUILayout.ObjectField("Animator Override", animatorOverride, typeof(AnimatorOverrideController), false));
            SetProjectile((Projectile)EditorGUILayout.ObjectField("Projectile", projectile, typeof(Projectile), false));
            EditorGUILayout.EndVertical();
        }

#endif
    }
}
