using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewGunConfigSO", menuName = "Configurations/Gun Configuration")]
    public class GunConfigSO : ScriptableObject
    {
        [Header("枪械名字")] public string gunName;
        [Header("射击间隔")] public float interval;
        [Header("枪械Transform偏移量")] public Vector2 transformOffset;
        [Header("枪械预制体")] public GameObject gunPrefab;
        [Header("子弹配置")] public BulletConfigSO bulletConfigSO;
    }
}
