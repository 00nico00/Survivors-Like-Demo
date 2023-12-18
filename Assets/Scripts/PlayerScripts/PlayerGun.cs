using System;
using Guns;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerScripts
{
    public class PlayerGun : MonoBehaviour
    {
        [SerializeField] private GameObject gunPrefab;
        [SerializeField] private Player player;

        private IGun _gun;
        private GameObject _currentGun;

        private void OnValidate()
        {
            if (gunPrefab != null)
            {
                var gun = gunPrefab.GetComponent<IGun>();
                if (gun == null)
                {
                    UnityEngine.Debug.LogError("提供的 gunPrefab 没有实现 IGun 接口");
                }
            }
        }

        private void Start()
        {
            ShowGun();
        }

        private void Update()
        {
            if (gunPrefab != null)
            {
                HandleGunDirection();
            }
        }

        /// <summary>
        /// 将 gunPrefab 挂载到
        /// </summary>
        private void ShowGun()
        {
            if (gunPrefab == null)
            {
                return;
            }

            _currentGun = Instantiate(gunPrefab, transform);
            _gun = _currentGun.GetComponent<IGun>();
            _currentGun.transform.localPosition =
                new Vector3(_gun.GunPara.transformOffset.x, _gun.GunPara.transformOffset.y, 0);
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