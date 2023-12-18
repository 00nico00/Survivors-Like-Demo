using System;
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
        public GunConfigSO GunPara => _gunConfigSO;

        [SerializeField] private GunConfigSO _gunConfigSO;
        [SerializeField] private Transform muzzle;

        private bool _isInCoolDown;

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
