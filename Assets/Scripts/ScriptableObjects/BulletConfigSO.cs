using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewBulletConfigSO", menuName = "Configurations/Bullet Configuration")]
    public class BulletConfigSO : ScriptableObject
    {
        [Header("子弹伤害")] public int bulletDamage;
        [Header("子弹速度")] public float bulletSpeed;
        [Header("子弹类型")] public BulletType bulletType;
        [Header("子弹预制体")] public GameObject bulletPrefab;
    }
    
    public enum BulletType
    {
        Normal,     // 普通
        Explosive,  // 击中会爆炸
    }
}
