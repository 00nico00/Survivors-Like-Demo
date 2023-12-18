using System;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void HandleWalkingAndIdleAnimation(bool isWalking)
        {
            _animator.SetBool(IsWalking, isWalking);
        }
    }
}
