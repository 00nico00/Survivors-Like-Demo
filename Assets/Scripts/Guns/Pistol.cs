using ScriptableObjects;
using UnityEngine;

namespace Guns
{
    public class Pistol : MonoBehaviour, IGun
    {
        public GunConfigSO GunPara => _gunConfigSO;

        [SerializeField] private GunConfigSO _gunConfigSO;
        [SerializeField] private Transform muzzle;
        
        public void Fire()
        {
            
        }
    }
}
