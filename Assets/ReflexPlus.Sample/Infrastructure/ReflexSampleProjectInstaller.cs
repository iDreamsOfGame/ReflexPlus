using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    public class ReflexSampleProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private PickupSoundEffect _pickupSoundEffectPrefab;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(_pickupSoundEffectPrefab);
        }
    }
}