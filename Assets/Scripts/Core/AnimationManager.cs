﻿using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// 这只是一个动画资源的wrapper
    /// 只是为了写起来方便，本意应该是一个加载动画资源的管理器
    /// </summary>
    public class AnimationManager : MonoBehaviour
    {
        public LinearMixerTransitionAsset.UnShared locomotion;
        public AnimationClip defaultAttack;
        public AnimationClip attack; // 攻击动画，会在运行时根据不同的武器做修改
        public AnimationClip death;

        private AnimancerComponent animancer;

        private void Awake()
        {
            animancer = GetComponent<AnimancerComponent>();
        }

        public void Move(float speed, bool interruptAttack = false)
        {
            if (animancer.IsPlaying(death)) return;
            if (animancer.IsPlaying(attack) && !interruptAttack) return;
            
            if (!animancer.IsPlaying(locomotion))
            {
                animancer.Play(locomotion, 0.25f);
            }

            locomotion.State.Parameter = speed;
        }

        private void StopAttackToLocomotion()
        {
            // 如果是攻击状态切换为移动状态，fade时间要短一些才好看
            animancer.Play(locomotion, 0.1f);
        }

        public void Die()
        {
            animancer.Play(death, 0.25f);
        }

        public void Attack()
        {
            if (animancer.IsPlaying(death)) return;
            animancer.Play(attack, 0.25f, FadeMode.FromStart).Events.OnEnd = StopAttackToLocomotion;
        }
    }
}