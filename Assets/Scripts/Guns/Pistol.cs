using System;
using Bullets;
using GameEvent;
using NicoFramework.Extensions;
using NicoFramework.Tools.EventCenter;
using NicoFramework.Tools.Timer;
using ScriptableObjects;
using UnityEngine;

namespace Guns
{
    public class Pistol : MonoBehaviour, IGun
    {
        public GunConfigSO GunConfig => _gunConfigSO;

        [SerializeField] private GunConfigSO _gunConfigSO;
        [SerializeField] private Transform muzzle;

        private bool _isInCoolDown;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var bullet = _gunConfigSO.bulletConfigSO.bulletPrefab.GetComponent<IBullet>();
            if (bullet == null)
            {
                Debug.Log("此 GunConfigSO 中的 bulletPrefab 没有实现 IBullet 接口");
            }
        }
#endif

        private void Start()
        {
            EventCenter.Default.Receive<InputEvent.AttackEvent>(_ =>
            {
                // 此处为了使得 Fire 和 Animation 同步消息触发，因此套了一层新的事件来统一触发两者
                OnAttackEvent();
            }).BindLifetime(this);

            EventCenter.Default.Receive<GunEvent.FireEvent>(_ => { Fire(); }).BindLifetime(this);
        }

        private void OnAttackEvent()
        {
            if (_isInCoolDown)
            {
                return;
            }

            TimerManager.Instance.CreateTimer()
                .SetDuration(TimeSpan.FromSeconds(_gunConfigSO.interval))
                .OnCrate(() => _isInCoolDown = true)
                .OnFinalLoopFinish(() => _isInCoolDown = false)
                .Register();
            
            // 继续触发 Fire 的事件
            EventCenter.Default.Publish(new GunEvent.FireEvent());
        }

        public void Fire()
        {
            Debug.Log("fire");
        }
    }
}