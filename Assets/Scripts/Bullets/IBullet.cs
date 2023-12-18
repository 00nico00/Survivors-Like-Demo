using ScriptableObjects;

namespace Bullets
{
    public interface IBullet
    {
        BulletConfigSO BulletConfig { get; }
    }
}