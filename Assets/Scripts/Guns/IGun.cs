using ScriptableObjects;

namespace Guns
{
    public interface IGun
    {
        GunConfigSO GunConfig { get; }
        bool IsInCoolDown { get; }

        void Fire();
    }
}
