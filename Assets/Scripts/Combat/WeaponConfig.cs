﻿using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private AnimationClip animationClip;
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

        public Weapon Spawn(Transform rightHand, Transform leftHand, AnimationManager animationManager)
        {
            DestroyOldWeapon(rightHand, leftHand);
            
            // 指定攻击动画为武器的动画，或者默认动画
            animationManager.attack = animationClip != null ? animationClip : animationManager.defaultAttack;

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

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return bonus;
            }
        }
    }
}