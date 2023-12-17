using ScriptableObjects;

namespace Guns
{
    public interface IGun
    {
        GunConfigSO GunPara { get; }

        void Fire();
    }
}
