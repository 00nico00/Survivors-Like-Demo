using System;
using NicoFramework.Tools.ObjectPool;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bullets
{
    public class PistolBullet : MonoBehaviour, IBullet, IAttackable
    {
        [SerializeField] private BulletConfigSO bulletConfig;

        private Rigidbody2D _rigidbody2D;
        
        public BulletConfigSO BulletConfig => bulletConfig;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"name: {other.name}, layer: {other.gameObject.layer}");
            // 攻击判定
            var target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                Attack(target);
            }
            
            // 回收子弹到对象池
            GameObjectPool.Instance.Store(gameObject);
        }

        public void InitOnFire(float scatterAngle, Vector2 direction)
        {
            _rigidbody2D.velocity = Quaternion.AngleAxis(scatterAngle, Vector3.forward) * direction *
                                    bulletConfig.bulletSpeed;
        }
        
        public void Attack(IDamageable target)
        {
            target.TakeDamage(bulletConfig.bulletDamage);
        }
    }
}