using System;
using NicoFramework.Extensions;
using NicoFramework.Tools.EventCenter;
using GameEvent;
using UnityEngine;

namespace Guns
{
    public class GunAnimation : MonoBehaviour
    {
        private Animator _animator;

        private static readonly int Fire = Animator.StringToHash("fire");
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            EventCenter.Default.Receive<GunEvent.FireEvent>(_ =>
            {
                PlayFireAnimation();
            }).BindLifetime(this);
        }

        private void PlayFireAnimation()
        {
            _animator.SetTrigger(Fire);
        }
    }
}
