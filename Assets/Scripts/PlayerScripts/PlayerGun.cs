using System;
using Guns;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerScripts
{
    public class PlayerGun : MonoBehaviour
    {
        [SerializeField] private GunConfigSO gunConfig;
        [SerializeField] private Player player;

        private IGun _gun;
        private GameObject _currentGun;

#if UNITY_EDITOR        
        private void OnValidate()
        {
            if (gunConfig != null)
            {
                var gun = gunConfig.gunPrefab.GetComponent<IGun>();
                if (gun == null)
                {
                    UnityEngine.Debug.LogError("提供的 gunConfig 没有实现 IGun 接口");
                }
            }
        }
#endif
        
        private void Start()
        {
            ShowGun();
        }

        private void Update()
        {
            if (gunConfig != null)
            {
                HandleGunDirection();
            }
        }

        /// <summary>
        /// 将 gunConfig 挂载到
        /// </summary>
        private void ShowGun()
        {
            if (gunConfig == null)
            {
                return;
            }

            _currentGun = Instantiate(gunConfig.gunPrefab, transform);
            _gun = _currentGun.GetComponent<IGun>();
            _currentGun.transform.localPosition =
                new Vector3(_gun.GunConfig.transformOffset.x, _gun.GunConfig.transformOffset.y, 0);
        }

        /// <summary>
        /// 根据鼠标位置处理枪械朝向
        /// </summary>
        private void HandleGunDirection()
        {
            var mousePos = GameInput.Instance.GetMousePosInWorldPoint();
            var direction = (mousePos -
                             new Vector2(_currentGun.transform.position.x,_currentGun.transform.position.y)).normalized;
            _currentGun.transform.right = direction;
            HandleGunFlip(direction);
        }

        /// <summary>
        /// 处理枪械位置翻转
        /// </summary>
        private void HandleGunFlip(Vector2 direction)
        {
            int flipX = player.IsFacingRight ? 1 : -1;
            int flipY = direction.x > 0f ? 1 : -1;

            _currentGun.transform.localScale = new Vector3(flipX, flipY, _currentGun.transform.localScale.z);
        }
    }
}