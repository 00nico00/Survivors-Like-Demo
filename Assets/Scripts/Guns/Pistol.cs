using System;
using Bullets;
using NicoFramework.Extensions;
using NicoFramework.Tools.EventCenter;
using NicoFramework.Tools.Timer;
using PlayerScripts;
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
                Fire();
            }).BindLifetime(this);
        }

        public void Fire()
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
        }
    }
}