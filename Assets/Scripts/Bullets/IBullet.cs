using ScriptableObjects;
using UnityEngine;

namespace Bullets
{
    public interface IBullet
    {
        BulletConfigSO BulletConfig { get; }

        void InitOnFire(float scatterAngle, Vector2 direction);
    }
}