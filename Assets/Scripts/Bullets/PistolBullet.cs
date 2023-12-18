using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bullets
{
    public class PistolBullet : MonoBehaviour, IBullet, IAttackable
    {
        [SerializeField] private BulletConfigSO bulletConfig;

        public BulletConfigSO BulletConfig => bulletConfig;


        public void Attack(IDamageable target)
        {
            target.TakeDamage(bulletConfig.bulletDamage);
        }
    }
}