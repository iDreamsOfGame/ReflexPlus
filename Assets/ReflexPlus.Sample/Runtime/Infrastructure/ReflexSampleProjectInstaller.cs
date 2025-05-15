using ReflexPlus.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReflexPlus.Sample.Infrastructure
{
    public class ReflexSampleProjectInstaller : MonoBehaviour, IInstaller
    {
        [FormerlySerializedAs("_pickupSoundEffectPrefab")]
        [SerializeField]
        private PickupSoundEffect pickupSoundEffectPrefab;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(pickupSoundEffectPrefab);
        }
    }
}