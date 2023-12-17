using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewGunConfigSO", menuName = "Configurations/Gun Configuration")]
    public class GunConfigSO : ScriptableObject
    {
        [Header("枪械名字")] public string gunName;
        [Header("射击间隔")] public float interval;
        [Header("伤害")] public int damage;
        [Header("枪械图片")] public Sprite gunSprite;
        [Header("子弹预制体")] public GameObject bulletPrefab;
        [Header("枪械Transform偏移量")] public Vector2 transformOffset;
    }
}
