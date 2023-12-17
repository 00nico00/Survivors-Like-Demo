using System;
using Guns;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerGun : MonoBehaviour
    {
        [SerializeField] private GameObject gunPrefab;
        
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
    }
}
