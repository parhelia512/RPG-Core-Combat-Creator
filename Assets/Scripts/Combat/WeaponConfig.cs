﻿using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatornOverride = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private float damage = 5f;
        [SerializeField] private float bonus = 50f;
        [SerializeField] private float range = 2f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string weaponName = "Weapon";
        public float Damage { get => damage; }
        public float Range { get => range; }
        public float Bonus { get => bonus; }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatornOverride != null)
            {
                animator.runtimeAnimatorController = animatornOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            Weapon weapon = null;
            if (null != equippedPrefab)
            {
                weapon = Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
                weapon.gameObject.name = weaponName;
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            // notice: rename before destroyed avoid from Frame BUG
            oldWeapon.name = "__DESTROYING__";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            var projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }
    }
}