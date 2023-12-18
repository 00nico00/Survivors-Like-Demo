using ScriptableObjects;

namespace Guns
{
    public interface IGun
    {
        GunConfigSO GunConfig { get; }

        void Fire();
    }
}
